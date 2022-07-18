#if UNITY_EDITOR
using UnityEngine;
using UnityCommon;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;
using System.Collections;

/// <summary>
/// 2020-10-20 화 오후 8:38:59, 4.0.30319.42000, YONG-PC, Yong
/// </summary>
namespace UnityCommon
{
   /// <summary>
   /// Not UnityEngine.Object type now..
   /// </summary>
    public partial class EditorTable<TRow>
    {
        IEnumerable<TRow> m_rows;
        List<ColumnBase> m_cols;
        Func<int, GUIStyle> m_rowStyle;
        Comparison<TRow> m_order;
        GUIStyle m_headerRowStyle;
      bool m_flexibleSpace;
      Vector2 m_scroll;

      public IEnumerable<TRow> Rows { get => m_rows; set => m_rows = value; }
        public List<ColumnBase> Columns { get => m_cols; set => m_cols = value; }
        public Func<int, GUIStyle> RowStyle { get => m_rowStyle; set => m_rowStyle = value; }
        public Comparison<TRow> Order { get => m_order; set => m_order = value; }
      public bool FlexibleSpace { get => m_flexibleSpace; set => m_flexibleSpace = value; }

        public EditorTable()
        {
            m_rows = new List<TRow>();
            m_cols = new List<ColumnBase>();
        }

        public void Add(ColumnBase col)
        {
            m_cols.Add(col);
        }

        public void AddColumns(params ColumnBase[] cols)
        {
            foreach (var col in cols)
            {
                m_cols.Add(col);
            }
        }

        public void OnGUI()
        {
         //var m_rows = m_order != null ? m_rows.OrderBy(m_order) : m_rows;
         int ridx = 0;

            SetDefaultStyle();

         m_scroll = EditorGUILayout.BeginScrollView(m_scroll);
         // Box
         GUILayout.BeginVertical();

            // Header
            BeginRow(-1);
            for (int i = 0; i < m_cols.Count; ++i)
            {
                var c = m_cols[i];

                // header cell
                BeginCell(c.Width, c.WidthExtand);
                c.DrawHeader(this);
                EndCell();

            }
            EndRow();

            // Each rows
            ridx = 0;
            try
            {
                foreach (var row in m_rows)
                {
                    BeginRow(ridx);

                    for (int i = 0; i < m_cols.Count; ++i)
                    {
                        var c = m_cols[i];

                        // cell
                        BeginCell(c.Width, c.WidthExtand);
                        c.DrawColumnCell(row);
                        EndCell();
                    }

                    EndRow();

                    ridx += 1;
                }
            }
            catch(InvalidOperationException)
            {
                // when collection modified
            }

            GUILayout.EndVertical();

         EditorGUILayout.EndScrollView();
      }

        void BeginRow(int rowIdx)
        {
            GUILayout.BeginHorizontal(m_rowStyle(rowIdx));
        }

        void EndRow()
        {
            GUILayout.EndHorizontal();
        }

        void BeginCell(int width, bool extend)
        {
            if (width >= 0 && !extend)
            {

                GUILayout.BeginVertical(GUILayout.Width(width));
            }
            else
            {
                GUILayout.BeginVertical();
            }

         if (m_flexibleSpace) GUILayout.FlexibleSpace();
        }

        void EndCell()
        {
            if (m_flexibleSpace) GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        void SetDefaultStyle()
        {
            if (m_rowStyle == null)
            {
                var row0 = new GUIStyle(GUI.skin.label);
                var row1 = new GUIStyle(GUI.skin.label);
                
                row1.normal.background = Texture2DTool.CreateTexture(1, 1, new Color(0.85f, 0.85f, 0.85f));
                row0.normal.background = Texture2DTool.CreateTexture(1, 1, new Color(0.85f, 0.85f, 0.95f));

                m_rowStyle = (x) => x % 2 == 0 ? row0 : row1;
            }
        }
    }
}
#endif