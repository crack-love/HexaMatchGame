using System.Collections.Generic;
using UnityEngine;

namespace YGK
{
   class SpinnerFigureCheck : FigureCheckBehaviour
   {
      [SerializeField] BlockEntity m_itemPrefab = null;
      
      // 다이아 좌상우상위, 좌하우하아래, 좌상좌하-2col, 우상우하+2col
      public override bool Check(Block src, HashSet<Block> figureds = null, List<ItemCreateInfo> items = null)
      {
         // TODO : use DP algorithm -> performance
         // TODO : list pool

         if (!src) return false;

         var map = BlockMap.Instance;
         var row = src.Row;
         var col = src.Col;
         var delata = HexaDirections.GetDelta(col);

         var lu = FigureCheckTool.GetBlock(src, HexaDirection.LeftUp);
         var ld = FigureCheckTool.GetBlock(src, HexaDirection.LeftDown);
         var u = FigureCheckTool.GetBlock(src, HexaDirection.Up);
         var d = FigureCheckTool.GetBlock(src, HexaDirection.Down);
         var ru = FigureCheckTool.GetBlock(src, HexaDirection.RightUp);
         var rd = FigureCheckTool.GetBlock(src, HexaDirection.RightDown);
         var cols2 = FigureCheckTool.GetBlock(src, -2);
         var cola2 = FigureCheckTool.GetBlock(src, +2);

         List<Block> fits = new List<Block>();
         if (fits.Count <= 0 && lu != null && ru != null && u != null)
         {
            fits.Add(lu);
            fits.Add(ru);
            fits.Add(u);
         }
         else if (fits.Count <= 0 && ld != null && rd != null && d != null)
         {
            fits.Add(ld);
            fits.Add(rd);
            fits.Add(d);
         }
         else if (fits.Count <= 0 && lu != null && ld != null && cols2 != null)
         {
            fits.Add(lu);
            fits.Add(ld);
            fits.Add(cols2);
         }
         else if (fits.Count <= 0 && ru != null && rd != null && cola2 != null)
         {
            fits.Add(ru);
            fits.Add(rd);
            fits.Add(cola2);
         }

         // item generated
         if (fits.Count > 0)
         {
            // add to set
            if (figureds != null)
            {
               foreach (Block b in fits)
               {
                  figureds.Add(b);
               }
            }

            // add item
            if (items != null)
            {
               items.Add(new ItemCreateInfo()
               {
                  Row = src.Row,
                  Col = src.Col,
                  ItemEntity = m_itemPrefab,
                  ColorLayer = src.ColorLayer,
                  Mergeds = fits,
               });
            }
         }

         return fits.Count > 0;
      }
   }
}