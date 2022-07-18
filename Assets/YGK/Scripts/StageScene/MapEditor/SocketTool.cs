#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityCommon;

namespace YGK.MapEditor
{
   // todo : create new socket, list/select socket
   class SocketTool : ToolbarItem
   {
      enum Mode
      {
         Enable, Disable, Replace
      }

      [SerializeField] Mode m_mode;
      [SerializeField] SocketEntity m_replaceEntity;
      [SerializeField] EditorWindow m_window;
      string[] m_modeNames;
      BlockMap m_map;

      public override string name => "Socket";
      
      public void SetWindow(EditorWindow window)
      {
         m_window = window;
      }

      public override void OnSelected()
      {
         m_map = BlockMap.Instance;
         m_modeNames = System.Enum.GetNames(typeof(Mode));
         SceneViewRaycaster.BeforeSceneGUI += SceneViewRaycast;
         HandleDrawer.SetEnable(true);
      }

      public override void OnDeselected()
      {
         SceneViewRaycaster.BeforeSceneGUI -= SceneViewRaycast;
         HandleDrawer.SetEnable(false);
      }

      public override void OnGUI()
      {
         m_mode = (Mode)GUILayout.Toolbar((int)m_mode, m_modeNames);

         if (m_mode == Mode.Replace)
         {
            m_replaceEntity = EditorGUILayout.ObjectField("Replace Entity", m_replaceEntity, typeof(SocketEntity), false) as SocketEntity;

            // inspect replace entity count
            int cnt = 0;
            foreach(var s in m_map.GetSocketEnumerable())
            {
               if (s.Entity == m_replaceEntity)
               {
                  ++cnt;
               }
            }

            EditorGUILayout.LabelField("Replace Entity Count : " + cnt.ToString());
            m_window?.Repaint(); // update window

         }
         else if (m_mode == Mode.Enable)
         {
            GUILayout.Label("Click Scene Socket to Enable");
         }
         else if (m_mode == Mode.Disable)
         {
            GUILayout.Label("Click Scene Socket to Disable");
         }

         GUILayout.FlexibleSpace();
         if (GUILayout.Button("Enable All"))
         {
            m_map.Entity.SetEnableAllSockets(true);
            m_map.ReloadBlocks();
         }

         if (GUILayout.Button("Disable All"))
         {
            m_map.Entity.SetEnableAllSockets(false);
            m_map.ReloadBlocks();
         }

         if (GUILayout.Button("Clear All"))
         {
            m_map.Entity.ClearAllSockets();
            m_map.ReloadBlocks();
         }
      }

      void SceneViewRaycast(SceneView view)
      {
         // screen to index
         Vector3 wpos = SceneViewRaycaster.ScreenPointToWorld(view);
         var (i, j) = m_map.WorldPosToIndex(wpos.x, wpos.y);
         if (m_map.IsIndexOutOfRange(i, j)) return;

         // mouse drag or click
         var e = Event.current;
         if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
         {
            if (m_mode == Mode.Enable)
            {
               m_map.Entity.SetSocketEnable(i, j, true);
               m_map.SetSocketEnable(i, j, true);
            }
            else if (m_mode == Mode.Disable)
            {
               m_map.Entity.SetSocketEnable(i, j, false);
               m_map.SetSocketEnable(i, j, false);
            }
            else if (m_mode == Mode.Replace)
            {
               m_map.Entity.SetSocketEntity(i, j, m_replaceEntity);
               m_map.SetSocketEntity(i, j, m_replaceEntity);
            }
            e.Use();
         }

         HandleDrawer.SetHandleIndex(i, j, (int)m_mode);
      }
   }

}
#endif