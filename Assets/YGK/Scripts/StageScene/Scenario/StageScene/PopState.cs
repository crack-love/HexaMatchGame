using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace YGK.Scenario.StageScene
{
   class PopState : State
   {

      /**
       * 팝
       * 팝 있으면 드랍 모드로
       * 팝 없으면 클리어 모드로
       * 
       * 
       * TODO?
       * 팝 시킬 때: 팝만 명령시키고
       * Goal, Score, DestroySelfObject, PopAnimation 등 수행은 블록에게 위임?
       **/

      [SerializeField] float m_itemMergeAnimationSec = 0.5f;
      [SerializeField] int m_popScore = 20;

      public override void Initialize()
      {

      }

      public override void BeginState()
      {
         StartPopAsync();
      }

      async void StartPopAsync()
      {
         bool isPoped = await TryPopAllAsync1();

         if (isPoped)
         {
            // 드랍 모드로
            StateManager.Instance.ChangeState(StateType.Drop);
         }
         else if (!isPoped)
         {
            // 클리어 체크 모드로
            StateManager.Instance.ChangeState(StateType.Clear);
         }
      }

      async Task<bool> TryPopAllAsync1()
      {
         var datas = StateDatas.Instance;
         Dictionary<Block, int> hpDeltaSet = new Dictionary<Block, int>();
         List<Block> items = new List<Block>();
         bool isPoped = false;

         // 절대 팝 셋 등록
         foreach (Block b in datas.AbsolutePopBlocks)
         {
            hpDeltaSet.Add(b, -b.HP);
         }
         datas.AbsolutePopBlocks.Clear();

         // 피겨 체크
         // HP델타, 아이템 생성 반환
         HashSet<Block> swapSet = datas.SwapPopChecks;
         HashSet<Block> dropSet = datas.DropPopChecks;
         List<ItemCreateInfo> itemInfo0 = new List<ItemCreateInfo>();
         FigureCheck(swapSet, hpDeltaSet, itemInfo0);
         FigureCheck(dropSet, hpDeltaSet, itemInfo0);
         swapSet.Clear();
         dropSet.Clear();

         // 생성할 아이템 선택
         ChooseItem(itemInfo0, out var itemInfo1);
         var itemInfo2 = new List<ItemCreateInfo>(itemInfo1);

         // 주변 영향 루프
         // 주변 영향은 한 프레임에 한번만 최초 감소하는 블럭만 적용
         HashSet<Block> frameVisit = new HashSet<Block>();
         HashSet<Block> popSet = new HashSet<Block>();
         bool affectChainLoop = true;
         while (affectChainLoop)
         {
            // 아이템 사용 루프
            bool itemChainLoop = true;
            while (itemChainLoop)
            {
               foreach (var pair in hpDeltaSet)
               {
                  frameVisit.Add(pair.Key);
               }

               // HP 델타 적용
               // 뉴 팝 블럭반환
               DecreaseHps(hpDeltaSet, out var popBlock);
               isPoped |= hpDeltaSet.Count > 0;
               hpDeltaSet.Clear();
               foreach (var b in popBlock)
               {
                  popSet.Add(b);
               }

               // 아이템 사용
               // HP Delta 반환
               CollectItemRange(popBlock, items, hpDeltaSet);
               itemChainLoop = hpDeltaSet.Count > 0;
            }

            // 아이템 사용 애니메이션
            await StartItemsAnimationAsync(items);
            items.Clear();

            // 아이템 생성 애니
            await StartItemGenerateAnimationAsync(itemInfo1);
            itemInfo1.Clear();

            // 팝 애니메이션
            await StartPopAnimationAsync(popSet);

            // 팝
            // 골 체크, affect 체크
            PopBlocks(popSet, frameVisit, hpDeltaSet);
            popSet.Clear();
            affectChainLoop = hpDeltaSet.Count > 0;
         }

         // 아이템 생성 (Lazy)
         GenerateItems(itemInfo2);

         return isPoped;
      }

      void FigureCheck(HashSet<Block> srcCheckSet, Dictionary<Block, int> dstSet, List<ItemCreateInfo> itemGenInfo)
      {
         HashSet<Block> hashSet = new HashSet<Block>();

         foreach (Block block in srcCheckSet)
         {
            bool canpop = FigureChecker.Instance.FigureCheck(block, hashSet, itemGenInfo);
         }

         foreach (Block b in hashSet)
         {
            if (!b) continue;

            if (!dstSet.ContainsKey(b))
            {
               dstSet.Add(b, -1);
            }
            else
            {
               dstSet[b] -= 1;
            }
         }
      }

      /// <summary>
      /// decrese hp, if reach to zero add to popSet
      /// </summary>
      void DecreaseHps(Dictionary<Block, int> hpDeltaSet, out List<Block> poped)
      {
         poped = new List<Block>();

         foreach (var pair in hpDeltaSet)
         {
            pair.Key.SetHP(pair.Key.HP + pair.Value);
            
            if (pair.Key.HP <= 0)
            {
               poped.Add(pair.Key);
            }
         }
      }

      /// <summary>
      /// 아이템 사용 범위 콜렉트
      /// </summary>
      void CollectItemRange(List<Block> src, List<Block> usedItems, Dictionary<Block, int> hpDeltaSet)
      {
         List<Block> collected = new List<Block>();

         foreach (Block block in src)
         {
            if (!block.Entity.IsItem) continue;
            if (usedItems.Contains(block)) continue;

            usedItems.Add(block);

            // 범위 확인
            var bhv = block.GetComponent<ItemBehaviour>();
            bhv.CollectRange(block, collected);

            // 델타 추가
            foreach (Block b in collected)
            {
               if (b != block)
               {
                  if (hpDeltaSet.ContainsKey(b))
                     hpDeltaSet[b] -= 1;
                  else
                     hpDeltaSet.Add(b, -1);
               }
            }
         }
      }

      /// <summary>
      /// 아이템 사용 애니메이션
      /// </summary>
      async Task StartItemsAnimationAsync(List<Block> items)
      {
         if (StateDatas.Instance.AsyncronouseItemAnimation)
         {
            List<Task> animations = new List<Task>();

            foreach (Block b in items)
            {
               var task = b.GetComponent<ItemBehaviour>()?.StartAnimation();
               animations.Add(task);
            }

            await Task.WhenAll(animations);
         }
         else
         {
            foreach (Block b in items)
            {
               var task = b.GetComponent<ItemBehaviour>()?.StartAnimation();
               await task;
            }
         }
      }

      async Task StartPopAnimationAsync(HashSet<Block> set)
      {
         List<Task> animations = new List<Task>();

         foreach (var b in set)
         {
            if (!b) continue;

            var task = b.GetComponent<BlockPopAnimation>()?.StartAnimationAsync();
            if (task != null)
            {
               animations.Add(task);
            }
         }

         await Task.WhenAll(animations);
      }

      /// <summary>
      /// 생성할 아이템 선택
      /// </summary>
      void ChooseItem(List<ItemCreateInfo> srcList, out List<ItemCreateInfo> dstList)
      {
         // todo pool
         HashSet<Block> visited = new HashSet<Block>();
         dstList = new List<ItemCreateInfo>();
         var map = BlockMap.Instance;

         foreach (var info in srcList)
         {
            var srcBlock = map.GetBlock(info.Row, info.Col);
            if (visited.Contains(srcBlock)) continue;

            bool isOk = true;
            foreach (var block in info.Mergeds)
            {
               if (visited.Contains(block))
               {
                  isOk = false;
                  break;
               }
            }

            if (isOk)
            {
               // set visited
               visited.Add(srcBlock);
               foreach (var block in info.Mergeds)
               {
                  visited.Add(block);
               }

               dstList.Add(info);
            }
         }
      }

      async Task StartItemGenerateAnimationAsync(List<ItemCreateInfo> infos)
      {
         var map = BlockMap.Instance;
         var animations = new List<Task>();

         foreach (var info in infos)
         {
            if (info.Mergeds == null) continue;

            Vector3 dstPos = map.IndexToWorldPos(info.Row, info.Col);

            // start gen animation
            foreach (var block in info.Mergeds)
            {
               var begPos = block.transform.position;
               var task = new Simation(begPos, dstPos, block.transform, Simation.EaseInOutCurve, m_itemMergeAnimationSec).StartAsync();

               // add list
               animations.Add(task);
            }
         }

         await Task.WhenAll(animations);
      }


      /// <summary>
      /// Affect around, goal animatoin. 주변 효과는 한번만 적용
      /// </summary>
      void PopBlocks(HashSet<Block> blocks, HashSet<Block> frameVisit, Dictionary<Block, int> hpDelta)
      {
         foreach(Block b in blocks)
         {
            if (!b) continue;

            PopSingle(b, frameVisit, hpDelta);
         }
      }

      void PopSingle(Block b, HashSet<Block> frameVisit, Dictionary<Block, int> hpDelta)
      {
         var map = BlockMap.Instance;
         int r = b.Row;
         int c = b.Col;

         // affect around
         if (b.Entity.CanAffectOther)
         {
            foreach (var (rd, cd) in HexaDirections.GetDelta(c))
            {
               int nr = r + rd;
               int nc = c + cd;
               if (map.IsIndexOutOfRange(nr, nc)) continue;
               if (map.IsIndexNotEnabled(nr, nc)) continue;
               var nb = map.GetBlock(nr, nc);
               if (nb == null) continue;

               // affect
               if (nb.Entity.CanAffected)
               {
                  if (frameVisit.Add(nb))
                  {
                     if (hpDelta.ContainsKey(nb))
                        hpDelta[nb] -= 1;
                     else
                        hpDelta.Add(nb, -1);
                  }
               }
            }
         }

         // affect socket
         var socket = map.GetSocket(r, c);
         if (socket.Entity.CanAffectedByAbove)
         {
            socket.SetHP(socket.HP - 1);
         }
         
         // score
         Stage.Instance.AddScore(m_popScore);

         BlockMap.Instance.RemoveBlock(b);
         BlockFactory.Instance.RemoveBlock(ref b);
      }

      void GenerateItems(List<ItemCreateInfo> itemsGens)
      {
         if (itemsGens.Count <= 0) return;

         var map = BlockMap.Instance;
         foreach (var info in itemsGens)
         {
            if (!info.ItemEntity) continue;

            var item = BlockFactory.Instance.GetBlock(info.ItemEntity);
            map.SetBlock(info.Row, info.Col, item);
            map.AddChildBlock(item);
            map.RePositionBlock(item);

            item.SetColorLayer(info.ColorLayer);
         }
      }
   }

   /*
   async Task<bool> TryPopAllAsync()
   {
      // 피겨 검사
      // hp 감소 -> 팝 -> 아이템 사용 -> hp 감소
      // 아이템 애니메이션
      // 팝 애니메이션
      // 오브젝트 정리

      var datas = StateDatas.Instance;
      HashSet<Block> swapSet = datas.SwapPopChecks;
      HashSet<Block> dropSet = datas.DropPopChecks;
      HashSet<Block> hpPopSet = datas.HpPopBlocks;
      HashSet<Block> absPopSet = datas.AbsolutePopBlocks;
      List<ItemCreateInfo> itemGenInfos = new List<ItemCreateInfo>();
      List<Task> animations = new List<Task>();
      bool isPoped = false;

      // 모양 체크 리스트 검사
      FigureCheck(swapSet, hpPopSet, itemGenInfos);
      FigureCheck(dropSet, hpPopSet, itemGenInfos);
      swapSet.Clear();
      dropSet.Clear();
      swapSet = null;
      dropSet = null;

      // HP 감소
      DecreaseHps(hpPopSet, absPopSet);

      // 아이템 사용
      DistributeItems(absPopSet, out var absPopItems, out var absPopBlocks);
      isPoped = absPopItems.Count > 0 || absPopBlocks.Count > 0;
      hpPopSet.Clear();
      absPopSet.Clear();
      hpPopSet = null;
      absPopSet = null;

      // 아이템 작동 애니메이션 수행
      await StartItemsAnimationAsync(absPopItems);

      // 생성 아이템 선택
      ChooseItem(itemGenInfos, out var absItemGenInfos);
      itemGenInfos = null;

      // 아이템 생성 애니메이션 수행
      StartItemGenerateAnimation(absItemGenInfos, animations);
      await Task.WhenAll(animations);
      animations.Clear();

      // 블럭팝 애니메이션 수행
      StartPopAnimation(absPopBlocks, animations);
      StartPopAnimation(absPopItems, animations);
      await Task.WhenAll(animations);
      animations.Clear();

      // 아이템, 블럭 데이터 팝
      // 골 체크, affect 체크
      PopBlocks(absPopBlocks);
      PopBlocks(absPopItems);

      // 아이템 생성
      GenerateItems(absItemGenInfos);

      return isPoped;
   }
   */
}