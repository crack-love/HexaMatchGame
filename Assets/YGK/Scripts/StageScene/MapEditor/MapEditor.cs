#if UNITY_EDITOR
using UnityCommon;
using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace YGK.MapEditor
{
   class MapEditor : EditorWindow
   {
      [SerializeField] Toolbar m_toolbar;

      [MenuItem("YGK/Map Editor")]
      static void Init()
      {
         MapEditor window = GetWindow<MapEditor>();
         window.Show();
      }
      
      private void OnEnable()
      {
         if (!m_toolbar)
         {
            m_toolbar = CreateInstance<Toolbar>();
            m_toolbar.Add<FileTool>();
            m_toolbar.Add<SocketTool>().SetWindow(this);
            m_toolbar.Add<BlockTool>();
            m_toolbar.Parent = this;
         }
      }
      
      private void OnDisable()
      {
         m_toolbar.OnDisable();
      }

      private void OnDestroy()
      {
         DestroyImmediate(m_toolbar);
      }

      void OnGUI()
      {
         if (!Application.isEditor && m_toolbar.Enabled)
         {
            m_toolbar.SetEnable(false);
         }
         else if (Application.isEditor && !m_toolbar.Enabled)
         {
            m_toolbar.SetEnable(true);
         }

         m_toolbar.OnGUI();
      }
   }
}
#endif