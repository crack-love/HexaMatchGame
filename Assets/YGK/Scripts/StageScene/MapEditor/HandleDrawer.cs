#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityCommon;

namespace YGK.MapEditor
{
   static class HandleDrawer
   {
      static bool m_isEnable = false;
      static int m_i = 0;
      static int m_j = 0;
      static int m_mode = 0;

      public static void SetEnable(bool enable)
      {
         if (!m_isEnable && enable)
         {
            SceneView.duringSceneGui += Draw;
            m_isEnable = true;
         }
         else if (m_isEnable && !enable)
         {
            SceneView.duringSceneGui -= Draw;
            m_isEnable = false;
         }
      }

      public static void SetHandleIndex(int i, int j, int mode)
      {
         m_i = i;
         m_j = j;
         m_mode = mode;
      }

      public static void Draw(SceneView view)
      {
         var map = BlockMap.Instance;

         // Draw Handle Gizmo
         if (m_mode == 0)
         {
            Handles.color = Color.green;
         }
         else if (m_mode == 1)
         {
            Handles.color = Color.red;
         }
         else if (m_mode == 2)
         {
            Handles.color = Color.yellow;
         }
         else if (m_mode == 3)
         {
            Handles.color = Color.blue;
         }

         var handlePos = map.IndexToWorldPos(m_i, m_j);
         Vector3 size = new Vector3(map.ColGap, map.RowGap, 1);
         Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
         Handles.DrawWireCube(handlePos, size);

         // (must repaint to update draws)
         view.Repaint();
      }
   }

}
#endif