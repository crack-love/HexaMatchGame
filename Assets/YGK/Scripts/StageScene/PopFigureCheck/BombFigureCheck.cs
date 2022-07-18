using System.Collections.Generic;
using UnityEngine;

namespace YGK
{
   /// <summary>
   /// 대각선 체크
   /// </summary>
   class BombFigureCheck : FigureCheckBehaviour
   {
      [SerializeField] int m_minCondition = 3;
      [SerializeField] BlockEntity m_bombItem = null;
      
      // 좌상 3 좌하 3, 우상3 우하3, 좌상3 우상3, 좌하3 우하3
      public override bool Check(Block src, HashSet<Block> figureds = null, List<ItemCreateInfo> items = null)
      {
         // TODO : use DP algorithm -> performance
         // TODO : list pool

         if (!src) return false;

         List<Block> leftUps = new List<Block>();
         List<Block> leftDowns = new List<Block>();
         List<Block> rightUps = new List<Block>();
         List<Block> rightDowns = new List<Block>();

         FigureCheckTool.CollectTo(src, HexaDirection.LeftUp, leftUps);
         FigureCheckTool.CollectTo(src, HexaDirection.LeftDown, leftDowns);
         FigureCheckTool.CollectTo(src, HexaDirection.RightUp, rightUps);
         FigureCheckTool.CollectTo(src, HexaDirection.RightDown, rightDowns);

         List<Block> fits = new List<Block>();
         int cntWithSrc = m_minCondition - 1;
         int fitListCnt = 0;
         if (leftUps.Count >= cntWithSrc)
         {
            fits.AddRange(leftUps);
            fitListCnt += 1;
         }
         if (leftDowns.Count >= cntWithSrc)
         {
            fits.AddRange(leftDowns);
            fitListCnt += 1;
         }
         if (rightUps.Count >= cntWithSrc)
         {
            fits.AddRange(rightUps);
            fitListCnt += 1;
         }
         if (rightDowns.Count >= cntWithSrc)
         {
            fits.AddRange(rightDowns);
            fitListCnt += 1;
         }

         // item generated
         if (fitListCnt >= 2)
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
                  ItemEntity = m_bombItem,
                  ColorLayer = src.ColorLayer,
                  Mergeds = fits,
               });
            }
         }

         return fitListCnt >= 2;
      }
   }
}