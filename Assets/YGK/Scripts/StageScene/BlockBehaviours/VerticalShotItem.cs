using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   class VerticalShotItem : ItemBehaviour
   {
      [SerializeField] GameObject m_begSprite;
      [SerializeField] GameObject m_endSprite;
      [SerializeField] float m_duration = 0.2f;
      Block m_block;

      private void OnEnable()
      {
         m_block = GetComponent<Block>();
      }

      public override void CollectRange(Block src, List<Block> collected)
      {
         var map = BlockMap.Instance;
         var nrow = m_block.Row;
         var ncol = m_block.Col;

         // to beg dir
         while (true)
         {
            nrow += 1;
            if (map.IsIndexOutOfRange(nrow, ncol)) break;
         }

         // to end dir
         while (true)
         {
            nrow -= 1;
            if (map.IsIndexOutOfRange(nrow, ncol)) break;
            if (map.IsIndexNotEnabled(nrow, ncol)) continue;
            var block = map.GetBlock(nrow, ncol);
            if (block == null) continue;

            collected.Add(block);
         }
      }

      public override async Task StartAnimation()
      {
         var map = BlockMap.Instance;
         float lengthRCSize = Mathf.Max(map.ColSize, map.RowSize);
         float length = Mathf.Max(map.RowGap, map.ColGap) * (lengthRCSize + 1);

         // get end0, end1 (each frag's end position)
         var begPos = m_block.transform.position;
         var end0 = begPos + Vector3.up * length;
         var end1 = begPos + Vector3.down * length;

         var obj0 = Instantiate(m_begSprite);
         var obj1 = Instantiate(m_endSprite);
         obj0.transform.position = begPos;
         obj1.transform.position = begPos;
         var color = ColorManager.Instance.GetColor(m_block.ColorLayer);
         obj0.GetComponentInChildren<SpriteRenderer>().color = color;
         obj1.GetComponentInChildren<SpriteRenderer>().color = color;
         obj0.GetComponentInChildren<TrailRenderer>().startColor = color;
         obj1.GetComponentInChildren<TrailRenderer>().startColor = color;

         begPos.z -= 1;
         end0.z -= 1;
         end1.z -= 1;
         var task0 = new Simation(begPos, end0, obj0.transform, m_duration).StartAsync();
         var task1 = new Simation(begPos, end1, obj1.transform, m_duration).StartAsync();

         await task0;
         await task1;

         Destroy(obj0);
         Destroy(obj1);
      }
   }
}