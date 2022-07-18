using System.Collections.Generic;
using UnityEngine;

namespace YGK
{
   /// <summary>
   /// 세로 체크
   /// </summary>
   class VerticalFigureCheck : FigureCheckBehaviour
   {
      [SerializeField] int m_minCondition = 3;
      [SerializeField] int m_itemCondition = 4;
      [SerializeField] BlockEntity m_item = null;

      private void OnValidate()
      {
         if (m_itemCondition < m_minCondition)
            m_itemCondition = m_minCondition;
      }

      public override bool Check(Block src, HashSet<Block> figs = null, List<ItemCreateInfo> items = null)
      {
         // todo pool
         List<Block> collect = new List<Block>();

         // 상->하
         FigureCheckTool.CollectFromTo(src, HexaDirection.Up, HexaDirection.Down, collect);

         if (figs != null && collect.Count >= m_minCondition)
         {
            foreach (Block b in collect)
            {
               figs.Add(b);
            }
         }

         // add item
         if (items != null && collect.Count >= m_itemCondition)
         {
            items.Add(new ItemCreateInfo()
            {
               Row = src.Row,
               Col = src.Col,
               ItemEntity = m_item,
               ColorLayer = src.ColorLayer,
               Mergeds = collect,
            });
         }

         return collect.Count >= m_minCondition; 
      }
   }
}