using UnityEngine;
using UnityCommon;
using System;

namespace YGK
{
   partial class BlockMap : MonoBehaviourSingletone<BlockMap>
   {
      /**
       * 에디터 전용
       */

#if UNITY_EDITOR

      public StageEntity Entity => m_entity;

      public void SetSocketEnable(int i, int j, bool v)
      {
         int sidx = SerialIndex(i, j);
         m_sockets[sidx].gameObject.SetActive(v);

         if (m_blocks[sidx])
            m_blocks[sidx].gameObject.SetActive(v);

         Execute.SetDirty(this);
      }

      public void SetSocketEntity(int i, int j, SocketEntity e)
      {
         ClearCreateAndPositionInitalSocket(i, j, e);

         Execute.SetDirty(this);
      }

      public void SetBlockEntity(int i, int j, BlockEntity e)
      {
         ClearCreateAndPositionInitalBlock(i, j, e);

         Execute.SetDirty(this);
      }

#endif
   }
}