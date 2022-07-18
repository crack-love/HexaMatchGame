using System;
using System.Collections.Generic;
using UnityEngine;
using UnityCommon;

namespace YGK
{
   class BlockFactory : MonoBehaviourSingletone<BlockFactory>
   {
      [SerializeField] BlockEntity m_default;

      public Block GetBlock(BlockEntity src)
      {
         if (!src)
         {
            src = m_default;
         }

         Block res = Instantiate(src.Prefab);
         res.SetEntity(src);
         return res;
      }

      internal void RemoveBlock(ref Block src)
      {
         if (src)
         {
            Execute.DestroyGameObject(ref src);
            src = null;
         }
      }
   }
}