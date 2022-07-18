#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityCommon;
using System.Collections.Generic;

namespace YGK.MapEditor
{
   class BlockTool : ToolbarItem
   {
      const string Dir = "Assets/YGK/Entities/Blocks";

      enum Mode
      {
         Replace, ItemColor,
      }

      [SerializeField] Mode m_mode;
      [SerializeField] BlockEntity m_replaceEntity;
      [SerializeField] ColorLayer m_setColor;
      [SerializeField] EditorWindow m_window;
      EditorTable<BlockEntity> m_table;
      List<BlockEntity> m_entities;
      string[] m_modeNames;
      BlockMap m_map;

      public override string name => "Block";

      public void SetWindow(EditorWindow window)
      {
         m_window = window;
      }

      public override void OnSelected()
      {
         m_map = BlockMap.Instance;
         m_modeNames = System.Enum.GetNames(typeof(Mode));

         // create table
         if (m_table == null)
         {
            m_table = new EditorTable<BlockEntity>();
            m_table.Add(new EditorTable<BlockEntity>.ColumnLabel()
            {
               HeaderText = "Name",
               Getter = (x) => x == null ? "None" : x.name,
               OnClicked = (x) => m_replaceEntity = x,
            });
            m_table.Add(new EditorTable<BlockEntity>.ColumnButton()
            {
               HeaderText = " ",
               OnClicked = (x) => m_replaceEntity = x,
               ButtonText = "Select",

               Width = 50,
            });
         }
         RefreshBlockList();

         SceneViewRaycaster.BeforeSceneGUI += SceneViewRaycast;
         HandleDrawer.SetEnable(true);
      }

      public override void OnDeselected()
      {
         SceneViewRaycaster.BeforeSceneGUI -= SceneViewRaycast;
         HandleDrawer.SetEnable(false);
      }

      void RefreshBlockList()
      {
         m_entities = AssetDatabaseEx.LoadAllAssetsAtDir<BlockEntity>(Dir);
         m_entities.Insert(0, null);

         m_table.Rows = m_entities;
      }

      public override void OnGUI()
      {
         m_mode = (Mode)GUILayout.Toolbar((int)m_mode, m_modeNames);

         // Replace Mode
         if (m_mode == Mode.Replace)
         {
            m_table.OnGUI();

            // inspect replace entity count
            int cnt = 0;
            foreach (var s in m_map.GetBlockEnumerable())
            {
               if (s.Entity == m_replaceEntity)
               {
                  ++cnt;
               }
            }

            m_replaceEntity = EditorGUILayout.ObjectField(m_replaceEntity, typeof(BlockEntity), false) as BlockEntity;

            EditorGUILayout.LabelField("Replace Entity Count : " + cnt.ToString());
            m_window?.Repaint(); // update window
         }
         // Item Color Mode
         else if (m_mode == Mode.ItemColor)
         {
            m_setColor = (ColorLayer)EditorGUILayout.EnumPopup(m_setColor);
         }

         GUILayout.FlexibleSpace();

         // clear all
         if (GUILayout.Button("Clear All"))
         {
            for (int i = 0; i < m_map.RowSize; ++i)
               for (int j = 0; j  < m_map.ColSize; ++j)
               {
                  m_map.SetBlockEntity(i, j, null);
                  m_map.SetBlock(i, j, null);
                  m_map.Entity.SetBlockEntity(i, j, null);
               }
         }
      }

      void SceneViewRaycast(SceneView view)
      {
         Vector3 wpos = SceneViewRaycaster.ScreenPointToWorld(view);
         var (i, j) = m_map.WorldPosToIndex(wpos.x, wpos.y);
         if (m_map.IsIndexOutOfRange(i, j)) return;

         // mouse drag or click
         var e = Event.current;
         if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
         {
            if (m_mode == Mode.Replace)
            {
               m_map.Entity.SetBlockEntity(i, j, m_replaceEntity);
               m_map.SetBlockEntity(i, j, m_replaceEntity);

               EditorUtility.SetDirty(m_map);
               EditorUtility.SetDirty(m_map.Entity);
               e.Use();
            }
            else if (m_mode == Mode.ItemColor)
            {
               m_map.GetBlock(i, j).SetColorLayer(m_setColor);
               m_map.Entity.SetItemColor(i, j, m_setColor);

               EditorUtility.SetDirty(m_map);
               EditorUtility.SetDirty(m_map.Entity);
               e.Use();
            }
         }

         HandleDrawer.SetHandleIndex(i, j, 0);
      }
   }
   
}
#endif