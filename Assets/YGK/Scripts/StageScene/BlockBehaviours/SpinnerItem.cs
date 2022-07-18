using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   class SpinnerItem : ItemBehaviour
   {
      [SerializeField] int m_targetCount = 2;
      [SerializeField] Block m_block;
      [SerializeField] GameObject m_fly;
      [SerializeField] float m_flyDuration = 0.5f;

      List<Block> m_targets;

      private void OnEnable()
      {
         m_block = GetComponent<Block>();
      }

      public override void CollectRange(Block src, List<Block> collected)
      {
         // 맵에서
         // 골 아이템 or 현재소켓이 골 && hp>0인 블럭 찾음
         // TODO : 퍼포먼스를 위해 맵에서 리스트 관리

         if (m_targets == null) m_targets = new List<Block>();
         m_targets.Clear();

         var map = BlockMap.Instance;
         int cnt = 0;
         foreach(var b in map.GetBlockEnumerable())
         {
            if (cnt == m_targetCount)
            {
               break;
            }

            if (b.Entity.IsGoal)
            {
               m_targets.Add(b);
               collected.Add(b);
               ++cnt;
            }
         }

         foreach(var s in map.GetSocketEnumerable())
         {
            if (cnt == m_targetCount)
            {
               break;
            }

            if (s.Entity.IsGoal && s.HP > 0)
            {
               var r = s.Row;
               var c = s.Col;
               var b = map.GetBlock(r, c);
               if (b)
               {
                  m_targets.Add(b);
                  collected.Add(b);
                  ++cnt;
               }
            }
         }
      }

      // 타켓에 날아가는 애니메이션
      public override async Task StartAnimation()
      {
         var map = BlockMap.Instance;
         List<Task> tasks = new List<Task>();
         List<GameObject> flys = new List<GameObject>();

         foreach(var b in m_targets)
         {
            if (!b) continue;

            var r = b.Row;
            var c = b.Col;
            var wpos = map.IndexToWorldPos(r, c);

            var fly = Instantiate(m_fly);
            var beg = transform.position;
            beg.z -= 1;
            fly.transform.position = beg;
            fly.GetComponent<SpriteRenderer>().sortingOrder = SpriteOrderManager.Instance.GetOrder("GoalFly");
            tasks.Add(new Simation(beg, wpos, fly.transform, m_flyDuration).StartAsync());
            flys.Add(fly);
         }

         await Task.WhenAll(tasks);
         foreach(var f in flys)
         {
            if (f)
            {
               var copy = f;
               UnityCommon.Execute.DestroyGameObject(ref copy);
            }
         }
      }
   }
}