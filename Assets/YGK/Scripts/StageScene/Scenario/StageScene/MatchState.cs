using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace YGK.Scenario.StageScene
{
   class MatchState : State
   {
      /*
       * 인풋 대기
       * 인풋 입력 완료
       * ~~~~~~~
       * 드래그시 스왑
       * 스왑 후 매칭 확인
       * - 아이템 머지
       * 매칭시 팝 모드로 전환
       * 매칭 불가시 다시 재스왑
       * ~~~~~~~
       * 클릭시 팝
       * 팝 모드로
       * ~~~~~~~
       * 인풋 대기
       * */

      [SerializeField] Camera m_cam = null;
      [SerializeField] float m_dragStartDst = 0.5f;
      [SerializeField] float m_swapAnimationSec = 0.4f;
      [SerializeField] bool m_canMoveWhenCountZero;

      public override void Initialize()
      {

      }

      public override void BeginState()
      {
         if (m_cam == null)
            m_cam = Camera.main;

          GetInputAsync().Watch();
      }

      void OnDrawZizmos()
      {
         Gizmos.DrawLine(beg, end- beg);
      }

      Vector2 beg = default;
      Vector2 end = default;

      async Task GetInputAsync()
      {
         bool isTouchPressed = false;

         while (enabled)
         {
            // 터치 누르는 중
            if (Input.GetMouseButton(0))
            {
               Vector2 spos = (Input.touchCount == 0) ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;
               Vector2 wpos = m_cam.ScreenToWorldPoint(spos);

               // 터치 시작인경우
               if (Input.GetMouseButtonDown(0))
               {
                  beg = wpos;
                  isTouchPressed = true;
               }

               end = wpos;
            }

            // 터치 업
            if (Input.GetMouseButtonUp(0) && isTouchPressed)
            {
               // 드래그 끝
               Vector2 posDelta = end - beg;
               if (posDelta.sqrMagnitude > m_dragStartDst)
               {
                  Block a = null;
                  Block b = null;
                  bool ok = TryGetBlocksFromDrag(beg, end, ref a, ref b);
                  if (ok && a.Entity.CanSwap && b.Entity.CanSwap)
                  {
                     if (m_canMoveWhenCountZero)
                     {
                        SwapAsync(a, b).Watch();
                        return; // input 끝
                     }
                     // (남은 무브 검사)
                     else if (Stage.Instance.RemainMove > 0)
                     {
                        SwapAsync(a, b).Watch();
                        return; // input 끝
                     }
                  }
                  // 드래그 실패
                  else
                  {
                     beg = end = default;
                  }
               }
               // 클릭
               else
               {
                  // todo 아이템 사용
                  // 
               }
            }

            await Task.Yield();
         }
      }

      // dst 위치에 새 블럭 생성(기존것 삭제) 또는 둘다 사용
      bool TryMerge(Block src, Block dst)
      {
         if (src.Entity.CanMerge && dst.Entity.CanMerge)
         {
            // check merges from manager
            // 

            // or use both
            StateDatas.Instance.AbsolutePopBlocks.Add(src);
            StateDatas.Instance.AbsolutePopBlocks.Add(dst);

            return true;
         }

         return false;
      }

      // 스왑
      // 매칭 확인
      // 재스왑
      async Task SwapAsync(Block src, Block dst)
      {
         var srcRow = src.Row;
         var srcCol = src.Col;
         var dstRow = dst.Row;
         var dstCol = dst.Col;
         var srcPos = src.transform.position;
         var dstPos = dst.transform.position;

         // 스왑
         var stdAni = new Simation(srcPos, dstPos, src.transform, Simation.EaseInOutCurve, m_swapAnimationSec);
         var dtsAni = new Simation(dstPos, srcPos, dst.transform, Simation.EaseInOutCurve, m_swapAnimationSec);
         stdAni.Start();
         dtsAni.Start();
         await stdAni.AsyncTask;
         await dtsAni.AsyncTask;
         BlockMap.Instance.SetBlock(srcRow, srcCol, dst);
         BlockMap.Instance.SetBlock(dstRow, dstCol, src);

         // 머지 확인
         if (TryMerge(src, dst))
         {
            // 무브카운트 감소
            Stage.Instance.AddRemainMoveDelta(-1);

            // 팝 모드로
            StateManager.Instance.ChangeState(StateType.Pop);
            return;
         }

         // 매칭 확인
         bool canPop = FigureChecker.Instance.CanPopSwapped(src, dst);
         if (canPop)
         {
            var datas = StateDatas.Instance;
            datas.SwapPopChecks.Add(src);
            datas.SwapPopChecks.Add(dst);

            // 무브카운트 감소
            Stage.Instance.AddRemainMoveDelta(-1);

            // 팝 모드로
            StateManager.Instance.ChangeState(StateType.Pop);
         }
         else
         {
            // 재스왑
            stdAni = new Simation(srcPos, dstPos, dst.transform, Simation.EaseInOutCurve, m_swapAnimationSec);
            dtsAni = new Simation(dstPos, srcPos, src.transform, Simation.EaseInOutCurve, m_swapAnimationSec);
            stdAni.Start();
            dtsAni.Start();
            await stdAni.AsyncTask;
            await dtsAni.AsyncTask;
            BlockMap.Instance.SetBlock(srcRow, srcCol, src);
            BlockMap.Instance.SetBlock(dstRow, dstCol, dst);

            // 인풋 모드로
            GetInputAsync().Watch();
         }
      }

      bool TryGetBlocksFromDrag(Vector2 beg, Vector2 end, ref Block src, ref Block dst)
      {
         var map = BlockMap.Instance;
         dst = null;

         // get src block
         var (row, col) = map.WorldPosToIndex(beg.x, beg.y);
         if (map.IsIndexOutOfRange(row, col)) return false;
         if (!map.IsIndexEnabled(row, col)) return false;
         src = map.GetBlock(row, col);
         if (src == null) return false;

         // get dst block
         // 모든 방향 중 가장 드래그 끝과 가까운 방향 선택
         var minDist = 1e9f;
         var delta = HexaDirections.GetDelta(col);
         for (int k = 0; k < delta.Count; ++k)
         {
            int nrow = row + delta[k].row;
            int ncol = col + delta[k].col;
            if (map.IsIndexOutOfRange(nrow, ncol)) continue;
            if (!map.IsIndexEnabled(nrow, ncol)) continue;
            var block = map.GetBlock(nrow, ncol);
            if (block == null) continue;

            var wpos = (Vector2)map.IndexToWorldPos(nrow, ncol);
            float dist = (end - wpos).sqrMagnitude;

            // 가까운 거리
            if (dist < minDist)
            {
               minDist = dist;
               dst = block;
            }
         }

         return dst != null;
      }
   }
}