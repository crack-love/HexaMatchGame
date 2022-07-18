
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace YGK
{
   [Obsolete]
   [CreateAssetMenu(fileName ="New Pop Entity ", menuName ="Entity/Pop Entity")]
   class PopEntity : ScriptableObject
   {
      [Serializable]
      enum StartCondition
      {
         Odd, Even, 
      }

      [SerializeField][Range(2, 10)] int m_rowSize;
      [SerializeField][Range(2, 10)] int m_colSize;
      [SerializeField] bool[] m_filter;
      [SerializeField] BlockEntity m_item;
      [SerializeField] StartCondition m_startConditon;

      public int RowSize => m_rowSize;
      public int ColSize => m_colSize;
      public BlockEntity Item => m_item;
      
      public bool GetFilter(int i, int j)
      {
         int sidx = Serial(i, j);
         return m_filter[sidx];
      }

      int Serial(int i, int j)
      {
         return i + j * m_rowSize;
      }

#if UNITY_EDITOR
      [CustomEditor(typeof(PopEntity))]
      class PopEntityDrawer : Editor
      {
         Vector2 m_scroll = default;

         public override void OnInspectorGUI()
         {
            serializedObject.Update();
            PopEntity target = serializedObject.targetObject as PopEntity;

            // Filter Size
            GUILayout.Label("Filter Size (Row, Col)");
            var rsize = target.m_rowSize;
            var csize = target.m_colSize;
            var newRowSize = EditorGUILayout.IntSlider(target.m_rowSize, 2, 20);
            var newColSize = EditorGUILayout.IntSlider(target.m_colSize, 2, 20);
            if (newRowSize < 2) newRowSize = 2;
            if (newColSize < 2) newColSize = 2;
            if (rsize != newRowSize || csize != newColSize || target.m_filter == null)
            {
               rsize = newRowSize;
               csize = newColSize;
               target.m_filter = new bool[rsize * csize];
            }
            target.m_rowSize = rsize;
            target.m_colSize = csize;

            // StartCondition
            GUILayout.Space(10);
            GUILayout.Label("Start Conditoin");
            target.m_startConditon = (StartCondition)EditorGUILayout.EnumPopup(target.m_startConditon);
            var startCon = target.m_startConditon;

            // Filter
            var height = EditorGUIUtility.singleLineHeight;

            GUILayout.Space(10);
            GUILayout.Label("Filter");
            m_scroll = GUILayout.BeginScrollView(m_scroll);
            for (int i = 0; i < rsize; ++i)
            {
               GUILayout.BeginHorizontal(GUILayout.Width(height * csize));
               for (int j = 0; j < csize; ++j)
               {
                  int sidx = target.Serial(i, j);

                  GUILayout.BeginVertical();

                  if (j % 2 == (int)target.m_startConditon)
                     GUILayout.Space(height);

                  target.m_filter[sidx] =
                     EditorGUILayout.Toggle(target.m_filter[sidx], GUILayout.Width(height));

                  GUILayout.EndVertical();
               }
               GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            if (GUILayout.Button("Clear Filter"))
            {
               target.m_filter = new bool[rsize * csize];
            }

            // Item
            GUILayout.Space(10);
            GUILayout.Label("Item");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_item"), new GUIContent());

            serializedObject.ApplyModifiedProperties();
         }
      }
#endif
   }
}