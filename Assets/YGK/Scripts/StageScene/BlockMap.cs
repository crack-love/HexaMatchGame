using UnityEngine;
using UnityCommon;
using System.Collections.Generic;

namespace YGK
{
   // TODO : block Create, Remove and using factory thing to change use this
   partial class BlockMap : MonoBehaviourSingletone<BlockMap>
   {
      [SerializeField] StageEntity m_entity;
      [SerializeField] Transform m_socketsHolder;
      [SerializeField] Transform m_blocksHolder;
      [SerializeField] BlockSocket[] m_sockets;
      [SerializeField] Block[] m_blocks; // block can be null
      List<BlockSocket> m_spawnSockets = new List<BlockSocket>();

      public float RowGap => m_entity.RowGap;
      public float ColGap => m_entity.ColGap;
      public int RowSize => m_entity.RowSize;
      public int ColSize => m_entity.ColSize;

      public IEnumerable<BlockSocket> SpawnSockets => m_spawnSockets;

      [InspectorButton("ReloadBlocks", "Reload Blocks From Stage Entity")]
      [SerializeField] bool btn0;
      public void ReloadBlocks()
      {
         if (!m_entity) return;

         // clear
         Execute.DestroyGameObject(ref m_socketsHolder);
         Execute.DestroyGameObject(ref m_blocksHolder);
         ValidateHolder();

         // create sockets
         m_sockets = new BlockSocket[RowSize * ColSize];
         for (int i = 0; i < RowSize; ++i)
         {
            for (int j = 0; j < ColSize; ++j)
            {
               ClearCreateAndPositionInitalSocket(i, j, m_entity.GetBlockSocketEntity(i, j));
            }
         }

         // create blocks
         m_blocks = new Block[RowSize * ColSize];
         for (int i = 0; i < RowSize; ++i)
         {
            for (int j = 0; j < ColSize; ++j)
            {
               ClearCreateAndPositionInitalBlock(i, j, m_entity.GetBlockEntity(i, j));
            }
         }

         // allign self position
         float ycenter = (RowSize - 1) * RowGap / 2f;
         float xcenter = (ColSize -1) * ColGap / 2f;
         transform.position = new Vector3(-xcenter, ycenter, transform.position.z);

         // list spawnSockets
         m_spawnSockets.Clear();
         foreach (var socket in m_sockets)
         {
            if (socket.Entity.IsSpawn)
            {
               m_spawnSockets.Add(socket);
            }
         }

         Execute.SetDirty(this);
      }

      public IEnumerable<Block> GetBlockEnumerable()
      {
         var rsize = RowSize;
         var csize = ColSize;

         for (int i = 0; i < rsize; ++i)
            for (int j = 0; j < csize; ++j)
            {
               if (IsIndexNotEnabled(i, j)) continue;
               var block = GetBlock(i, j);
               if (block == null) continue;

               yield return block;
            }
      }

      public IEnumerable<BlockSocket> GetSocketEnumerable()
      {
         var rsize = RowSize;
         var csize = ColSize;

         for (int i = 0; i < rsize; ++i)
            for (int j = 0; j < csize; ++j)
            {
               if (IsIndexNotEnabled(i, j)) continue;
               var socket = GetSocket(i, j);
               if (socket == null) continue;

               yield return socket;
            }
      }

      public BlockSocket GetSocket(int i, int j)
      {
         int sidx = SerialIndex(i, j);
         return m_sockets[sidx];
      }

      /// <summary>
      /// Reload Blocks
      /// </summary>
      public void SetEntity(StageEntity e)
      {
         m_entity = e;
         ReloadBlocks();
         Execute.SetDirty(this);
      }

      public bool IsIndexOutOfRange(int i, int j)
      {
         return i < 0 || j < 0 || i >= RowSize || j >= ColSize;
      }

      public bool IsIndexNotEnabled(int i, int j)
      {
         return !m_entity.GetBlockSocketEnabled(i, j);
      }

      public bool IsIndexEnabled(int i, int j)
      {
         return m_entity.GetBlockSocketEnabled(i, j);
      }

      public Block GetBlock(int i, int j)
      {
         int idx = SerialIndex(i, j);
         return m_blocks[idx];
      }

      public void CreateNewBlock(int i, int j, BlockEntity e)
      {
         int sidx = SerialIndex(i, j);

         // clear prev block
         BlockFactory.Instance.RemoveBlock(ref m_blocks[sidx]);

         if (e != null)
         {
            //create
            var adding = BlockFactory.Instance.GetBlock(e);
            adding.transform.parent = m_blocksHolder;
            adding.SetIndex(i, j);
            m_blocks[sidx] = adding;

            // position
            var (x, y) = IndexToLocalPos(i, j);
            adding.transform.localPosition = new Vector3(x, y, 0);

            // set active
            adding.gameObject.SetActive(true);
         }
      }

      public void SetBlock(int i, int j, Block block)
      {
         int sidx = SerialIndex(i, j);
         m_blocks[sidx] = block;

         if (block != null)
         {
            block.SetIndex(i, j);
         }

         Execute.SetDirty(this);
      }

      public void AddChildBlock(Block block)
      {
         block.transform.parent = m_blocksHolder;
      }

      /// <summary>
      /// Remove from grid
      /// </summary>
      public void RemoveBlock(Block b)
      {
         int sidx = SerialIndex(b.Row, b.Col);
         m_blocks[sidx] = null;
      }

      void ValidateHolder()
      {
         if (!m_socketsHolder)
         {
            GameObject o = new GameObject("Sockets");
            m_socketsHolder = o.transform;
            m_socketsHolder.parent = transform;
            m_socketsHolder.transform.localPosition = new Vector3(0,0,1);
         }
         if (!m_blocksHolder)
         {
            GameObject o = new GameObject("Blocks");
            m_blocksHolder = o.transform;
            m_blocksHolder.parent = transform;
            m_blocksHolder.transform.localPosition = new Vector3(0, 0, 0);
         }
      }

      public void RePositionBlock(Block b)
      {
         // position
         var (x, y) = IndexToLocalPos(b.Row, b.Col);
         b.transform.localPosition = new Vector3(x, y, 0);
      }

      void ClearCreateAndPositionInitalBlock(int i, int j, BlockEntity e)
      {
         int sidx = SerialIndex(i, j);

         // clear prev block
         BlockFactory.Instance.RemoveBlock(ref m_blocks[sidx]);

         // create
         if (e != null)
         {
            var adding = BlockFactory.Instance.GetBlock(e);
            adding.transform.parent = m_blocksHolder;
            adding.SetIndex(i, j);
            m_blocks[sidx] = adding;

            // position
            var (x, y) = IndexToLocalPos(i, j);
            adding.transform.localPosition = new Vector3(x, y, 0);

            // set active
            adding.gameObject.SetActive(IsIndexEnabled(i, j));


            // item color setting
            if (e.IsItem)
            {
               adding.SetColorLayer(m_entity.GetItemColor(i,j));
            }
            // render color
            if (e.RenderColorName != null && e.RenderColorName.Length > 0)
            {
               adding.SetRenderColorByName(e.RenderColorName);
            }
         }
      }

      void ClearCreateAndPositionInitalSocket(int i, int j, SocketEntity entity)
      {
         int sidx = SerialIndex(i, j);

         // clear prev socket
         BlockSocketFactory.Instance.RemoveBlockSocket(ref m_sockets[sidx]);

         // create
         var socket = BlockSocketFactory.Instance.GetBlockSocket(entity);
         socket.transform.parent = m_socketsHolder;
         socket.SetIndex(i, j);
         m_sockets[sidx] = socket;

         // position
         var (x, y) = IndexToLocalPos(i, j);
         socket.transform.localPosition = new Vector3(x, y, 0);

         // color
         socket.SetColor(socket.Entity.ColorName);

         // set active
         socket.gameObject.SetActive(IsIndexEnabled(i, j));
      }

      int SerialIndex(int i, int j)
      {
         return i + RowSize * j;
      }

      public Vector3 IndexToWorldPos(int i, int j)
      {
         var (x, y) = IndexToLocalPos(i, j);

         return transform.TransformPoint(new Vector3(x, y, 0));
      }

      public (int, int) WorldPosToIndex(float x, float y)
      {
         var rgap = RowGap;
         var cgap = ColGap;

         Vector3 localPos = transform.InverseTransformPoint(new Vector3(x, y));
         int j = Mathf.RoundToInt(localPos.x / cgap);
         float i0 = -localPos.y / rgap;
         if (j % 2 == 1)
         {
            i0 -= rgap / 2f;
         }
         int i = Mathf.RoundToInt(i0);

         return (i, j);
      }

      (float, float) IndexToLocalPos(int i, int j)
      {
         var rgap = RowGap;
         var cgap = ColGap;

         float x = j * cgap;
         float y = -i * rgap;
         if (j % 2 == 1)
         {
            y -= rgap / 2f;
         }

         return (x,y);
      }
   }
}