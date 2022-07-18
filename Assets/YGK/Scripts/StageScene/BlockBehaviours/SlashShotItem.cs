using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   class SlashShotItem : ItemBehaviour
   {
      [SerializeField] HexaDirection m_begDir;
      [SerializeField] HexaDirection m_endDir;
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
            var delta = HexaDirections.GetDelta(ncol);
            nrow += delta[(int)m_begDir].row;
            ncol += delta[(int)m_begDir].col;
            if (map.IsIndexOutOfRange(nrow, ncol)) break;
         }

         // to end dir
         while (true)
         {
            var delta = HexaDirections.GetDelta(ncol);
            nrow += delta[(int)m_endDir].row;
            ncol += delta[(int)m_endDir].col;
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
         float length = Mathf.Max(map.RowGap, map.ColGap) * lengthRCSize;

         // get end0, end1 (each frag's end position)
         var begPos = m_block.transform.position;
         var delta = HexaDirections.GetDelta(m_block.Col);
         var nr = m_block.Row + delta[(int)m_begDir].row;
         var nc = m_block.Col + delta[(int)m_begDir].col;
         var dir = map.IndexToWorldPos(nr, nc) - begPos;
         var end0 = begPos + dir * length;
         nr = m_block.Row + delta[(int)m_endDir].row;
         nc = m_block.Col + delta[(int)m_endDir].col;
         dir = map.IndexToWorldPos(nr, nc) - begPos;
         var end1 = begPos + dir * length;

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