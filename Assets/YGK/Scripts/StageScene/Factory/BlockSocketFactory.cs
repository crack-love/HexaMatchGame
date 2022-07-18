using System;
using System.Collections.Generic;
using UnityEngine;
using UnityCommon;

namespace YGK
{
   class BlockSocketFactory : MonoBehaviourSingletone<BlockSocketFactory>
   {
      [SerializeField] SocketEntity m_default;

      public BlockSocket GetBlockSocket(SocketEntity e)
      {
         if (!e)
         {
            e = m_default;
         }

         BlockSocket s = Instantiate(e.Prefab);
         s.SetEntity(e);
         return s;
      }

      public void RemoveBlockSocket(ref BlockSocket src)
      {
         if (src)
         {
            Execute.DestroyGameObject(ref src);
            src = null;
         }
      }
   }
}