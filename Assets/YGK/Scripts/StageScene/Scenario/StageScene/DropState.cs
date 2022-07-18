using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YGK.Scenario.StageScene
{
   class DropState : State
   {
      /*
       * 1. 스폰에 생성 가능시 생성
       * 2. 하나씩 전부 드랍 가능 확인 (아래서부터)
       *  - 좌우로 내려오는 블럭은 목표지 위 라인에 블럭이 있으면 패스한다.
       * 3. 한 프레임에 한 칸씩 드랍 
       * 4. 모든 드랍 완료시 팝 모드로 (모든 블럭 드랍 애니메이션 종료, 모든 블럭 생성 종료)
       * 
       * 애니메이션 딜레이만큼 주기 반복
       * 
       * 
       * */

      [SerializeField] float m_blockDropSec; // Animation's duration

      HashSet<Block> m_movedSet = new HashSet<Block>();

      public override void Initialize()
      {

      }

      public override void BeginState()
      {
         DropCycleAsync();
      }

      async void DropCycleAsync()
      {
         var map = BlockMap.Instance;
         var data = StateDatas.Instance;

         bool isDropped = true;
         while (isDropped && this && enabled)
         {
            // drop blocks
            DropBlocks(ref isDropped);

            // spawn new blocks
            CreateNewBlocks(ref isDropped);

            if (isDropped)
            {
               // wait for next cycle
               await Task.Delay((int)(m_blockDropSec * 1000));
            }
         }

         // queue moved blocks to popmode
         foreach (Block b in m_movedSet)
         {
            data.DropPopChecks.Add(b);

            // TODO add animation of drop finished
            // 
         }
         m_movedSet.Clear();

         // animation time for drop finished
         await Task.Delay((int)(m_blockDropSec * 1000));
         if (!this || !enabled) return;

         // 팝 모드로
         StateManager.Instance.ChangeState(StateType.Pop);
      }

      void CreateNewBlocks(ref bool isDropped)
      {
         var map = BlockMap.Instance;
         var stage = Stage.Instance;

         // 새 블럭 생성
         // 스폰 블럭 아래 비어있을시 생성
         foreach(var socket in map.SpawnSockets)
         {
            var row = socket.Row;
            var col = socket.Col;
            var nrow = row + 1;
            if (!map.IsIndexEnabled(row, col)) continue;
            if (map.IsIndexOutOfRange(nrow, col)) continue;
            if (!map.IsIndexEnabled(nrow, col)) continue;
            var belowBlock = map.GetBlock(nrow, col);
            if (belowBlock != null) continue;

            // create
            var entity = stage.GetNextDropBlock();
            map.CreateNewBlock(row, col, entity); // pos is socket idx
            var block = map.GetBlock(row, col);

            // drop created block
            isDropped = TryDropSingle(block) | isDropped;
         }
      }

      void DropBlocks(ref bool isDropped)
      {
         var map = BlockMap.Instance;
         isDropped = false;

         // 드랍 체크
         for (int i = map.RowSize - 1; i >= 0; --i)
         {
            for (int j = 0; j < map.ColSize; ++j)
            {
               if (!map.IsIndexEnabled(i, j)) continue;
               var block = map.GetBlock(i, j);
               if (block == null) continue;

               isDropped = TryDropSingle(block) | isDropped;
            }
         }
      }
      
      bool TryDropSingle(Block block)
      {
         if (!block) return false;
         if (!block.Entity.CanDrop) return false;

         var map = BlockMap.Instance;
         int row = block.Row;
         int col = block.Col;
         var dirs = HexaDirections.DownDirection;
         var deltas = HexaDirections.GetDelta(col);

         foreach (var d in dirs)
         {
            int nr = row + deltas[(int)d].row;
            int nc = col + deltas[(int)d].col;
            if (map.IsIndexOutOfRange(nr, nc)) continue;
            if (!map.IsIndexEnabled(nr, nc)) continue;
            if (map.GetBlock(nr, nc) != null) continue;

            // (좌우로 떨어지는 블럭은 위에 블럭이 있으면 제외) (또는 스폰일시)
            if (d == HexaDirection.LeftDown || d == HexaDirection.RightDown)
            {
               var canNotDrop = false;
               var deltas2 = HexaDirections.GetDelta(nc);
               int nnr = nr;
               while (true)
               {
                  nnr = nnr - 1;
                  if (map.IsIndexOutOfRange(nnr, nc)) break;
                  if (map.IsIndexNotEnabled(nnr, nc)) break;

                  // 좌우 위에 스폰일시
                  if (map.GetSocket(nnr, nc).Entity.IsSpawn)
                  {
                     canNotDrop = true;
                     break;
                  }

                  // 위에 블럭이 있으면 (드랍중인)
                  var aboveBlock = map.GetBlock(nnr, nc);
                  if (aboveBlock != null)
                  {
                     if (aboveBlock.Entity.CanDrop)
                     {
                        canNotDrop = true;
                        break;
                     }
                     else
                     {
                        // 드랍 가능
                        canNotDrop = false;  
                        break;
                     }
                  }
               }

               if (canNotDrop)
               {
                  continue;
               }
            }

            // 애니메이션
            Vector2 from = map.IndexToWorldPos(row, col);
            Vector2 to = map.IndexToWorldPos(nr, nc);
            var ani = new Simation(to, block.transform, m_blockDropSec);
            ani.Start();

            // set grid
            map.SetBlock(row, col, null);
            map.SetBlock(nr, nc, block);

            // add to set
            m_movedSet.Add(block);

            return true;
         }

         return false;
      }
   }
}