using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   class BombItem : ItemBehaviour
   {
      [SerializeField] float m_duration = 0.2f;
      [SerializeField] Block m_block;

      private void OnEnable()
      {
         m_block = GetComponent<Block>();
      }

      public override void CollectRange(Block src, List<Block> collected)
      {
         var map = BlockMap.Instance;
         var row = m_block.Row;
         var col = m_block.Col;

         // around
         foreach(var del in HexaDirections.GetDelta(col))
         {
            var nrow = row + del.row;
            var ncol = col + del.col;
            if (map.IsIndexOutOfRange(nrow, ncol)) continue;
            if (map.IsIndexNotEnabled(nrow, ncol)) continue;
            var b = map.GetBlock(nrow, ncol);
            if (b == null) continue;

            collected.Add(b);
         }
      }

      public override async Task StartAnimation()
      {

      }
   }
}