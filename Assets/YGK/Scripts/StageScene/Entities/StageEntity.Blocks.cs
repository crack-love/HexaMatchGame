using System.Collections;
using System.Collections.Generic;
using UnityCommon;
using UnityEngine;

namespace YGK
{
   partial class StageEntity
   {
      // blocks
      [SerializeField] List<BlockEntity> m_blocks;
      [SerializeField] List<SocketEntity> m_sockets;
      [SerializeField] List<bool> m_socketEnables;
      [SerializeField] List<ColorLayer> m_itemColors;
      [SerializeField] [Range(2, 20)] int m_rowSize = 10;
      [SerializeField] [Range(2, 20)] int m_colSize = 7;
      [SerializeField] float m_rowGap = 0.7f;
      [SerializeField] float m_colGap = 0.6f;

      public int RowSize => m_rowSize;
      public int ColSize => m_colSize;
      public float RowGap => m_rowGap;
      public float ColGap => m_colGap;

      public bool IsEditorSceneNeedUpdate
      {
         get; set;
      }

      int m_lastRowSize;
      int m_lastColSize;
      float m_lastRowGap;
      float m_lastColGap;
      public void OnValidate()
      {
         ValidateSize(ref m_socketEnables, true);
         ValidateSize(ref m_sockets, null);
         ValidateSize(ref m_blocks, null);
         ValidateSize(ref m_itemColors, ColorLayer.None);

         if (m_lastRowSize != m_rowSize ||
            m_lastColSize != m_colSize ||
            m_rowGap != m_lastRowGap ||
            m_colGap != m_lastColGap)
         {
            m_lastRowSize = m_rowSize;
            m_lastColSize = m_colSize;
            m_lastRowGap = m_rowGap;
            m_lastColGap = m_colGap;

            IsEditorSceneNeedUpdate = true;
         }
      }

      void ValidateSize<T>(ref List<T> list, T defaultValue)
      {
         if (list == null) list = new List<T>();

         int ssize = list.Count;
         while (ssize++ < m_rowSize * m_colSize)
         {
            list.Add(defaultValue);
         }

         ssize = list.Count;
         while (ssize-- > m_rowSize * m_colSize)
         {
            list.RemoveAt(list.Count - 1);
         }
      }

      public void SetEnableAllSockets(bool value)
      {
         for (int i = 0; i < m_rowSize; ++i)
            for (int j = 0; j < m_colSize; ++j)
            {
               SetSocketEnable(i, j, value);
            }

         Execute.SetDirty(this);
      }

      public void ClearAllSockets()
      {
         for (int i = 0; i < m_rowSize; ++i)
            for (int j = 0; j < m_colSize; ++j)
            {
               int sidx = Serial(i, j);
               m_sockets[sidx] = null;
            }

         Execute.SetDirty(this);
      }

      public void SetSocketEntity(int i, int j, SocketEntity e)
      {
         m_sockets[Serial(i, j)] = e;

         Execute.SetDirty(this);
      }

      public void SetBlockEntity(int i, int j, BlockEntity e)
      {
         m_blocks[Serial(i, j)] = e;

         Execute.SetDirty(this);
      }

      public void SetSocketEnable(int i, int j, bool v)
      {
         m_socketEnables[Serial(i, j)] = v;

         Execute.SetDirty(this);
      }

      public void SetItemColor(int i, int j, ColorLayer c)
      {
         m_itemColors[Serial(i, j)] = c;

         Execute.SetDirty(this);
      }

      public ColorLayer GetItemColor(int i, int j)
      {
         return m_itemColors[Serial(i, j)];
      }

      public BlockEntity GetBlockEntity(int i, int j)
      {
         return m_blocks[i + m_rowSize * j];
      }

      public SocketEntity GetBlockSocketEntity(int i, int j)
      {
         return m_sockets[Serial(i, j)];
      }

      public bool GetBlockSocketEnabled(int i, int j)
      {
         return m_socketEnables[Serial(i, j)];
      }

      int Serial(int i, int j)
      {
         return i + m_rowSize * j;
      }
   }
}