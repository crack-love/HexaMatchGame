using System.Collections.Generic;
using UnityEngine;

namespace YGK
{
   class FigureChecker : UnityCommon.MonoBehaviourSingletone<FigureChecker>
   {
      // 순서가 아이템 생성 우선순위
      [SerializeField] FigureCheckBehaviour[] m_popCheckers;

      /// <summary>
      /// 2개 스왑된 블럭 체크 (가능한지)
      /// </summary>
      public bool CanPopSwapped(Block b0, Block b1)
      {
         if (AnyFigureCheck(b0))
         {
            return true;
         }
         else if (AnyFigureCheck(b1))
         {
            return true;
         }

         return false;
      }

      bool AnyFigureCheck(Block src)
      {
         foreach (var checker in m_popCheckers)
         {
            if (checker.Check(src))
            {
               return true;
            }
         }
         return false;
      }

      /// <summary>
      /// 블럭이용 주변 피겨 체크
      /// </summary>
      public bool FigureCheck(Block src, HashSet<Block> affecteds = null, List<ItemCreateInfo> items = null)
      {         
         bool isFigured = false;
         foreach(var checker in m_popCheckers)
         {
            bool ok = checker.Check(src, affecteds, items);
            isFigured |= ok;
         }

         return isFigured;
      }
   }
}