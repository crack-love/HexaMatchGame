#if UNITY_EDITOR
using UnityCommon;
using UnityEditor;

using UnityEngine;

/// <summary>
/// 2021-06-08 화 오후 4:39:19, 4.0.30319.42000, YONG-PC, Yong
/// </summary>
namespace YGK
{
   partial class BootManager
   {
      [CustomEditor(typeof(BootManager))]
      class Inspector : Editor
      {
         const int colWidth = 100;

         GUILayoutOption[] m_widthOptions;
         bool[] m_sortOrders;
         string[] m_colNames;
         int m_nowSortingIdx = 1;

         private void OnEnable()
         {
            m_widthOptions = new GUILayoutOption[]
            {
               GUILayout.ExpandWidth(true),
               GUILayout.Width(colWidth),
               GUILayout.Width(colWidth),
            };

            m_colNames = new string[]
            {
               "Name",
               "Start Order",
               "Final Order",
            };

            m_sortOrders = new bool[3];
         }

         public override void OnInspectorGUI()
         {
            var target = base.target as BootManager;

            // Colume headers
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < 3; ++i)
            {
               if (GUILayout.Button(GetColName(i), m_widthOptions[i]))
               {
                  m_nowSortingIdx = i;
                  m_sortOrders[i] = !m_sortOrders[i];

                  if (i == 0)
                  {
                     target.SortAsName(m_sortOrders[i]);
                  }
                  else if (i == 1)
                  {
                     target.SortAsStart(m_sortOrders[i]);
                  }
                  else
                  {
                     target.SortAsFinal(m_sortOrders[i]);
                  }
               }
            }
            EditorGUILayout.EndHorizontal();

            target.Validation();

            // Rows
            foreach (var token in target.m_managers)
            {
               EditorGUILayout.BeginHorizontal();
               EditorGUI.BeginChangeCheck();
               EditorGUILayout.LabelField(token.Mono.name, m_widthOptions[0]);
               token.StartOrder = EditorGUILayout.IntField(token.StartOrder, m_widthOptions[1]);
               token.FinalOrder = EditorGUILayout.IntField(token.FinalOrder, m_widthOptions[2]);
               if (EditorGUI.EndChangeCheck())
               {
                  Execute.SetDirty(target);
               }
               EditorGUILayout.EndHorizontal();
            }
         }

         string GetColName(int idx)
         {
            if (m_nowSortingIdx == idx)
            {
               return m_colNames[idx] + (m_sortOrders[idx] ? " △" : " ▽");
            }
            else
            {
               return m_colNames[idx];
            }
         }
      }
   }
}
#endif