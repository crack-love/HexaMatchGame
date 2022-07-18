#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 2020-12-07 월 오후 3:45:24, 4.0.30319.42000, YONG-PC, Yong
/// </summary>
namespace UnityCommon
{
   public partial class EditorTable<TRow>
   {
      public abstract class ColumnBase
      {
         public static int DefaultFieldWidth { get; set; } = 90;
         public static int DefaultButtonWidth { get; set; } = 22;
         public string HeaderText { get; set; }
         public Action OnHeaderClicked { get; set; }
         public Comparison<TRow> OnHeaderClickedSorter { get; set; }
         public int Width { get; set; } = DefaultFieldWidth;
         public bool WidthExtand { get; set; }
         public bool Enabled { get; set; } = true;

         bool m_reverseSort;

         public virtual void DrawHeader(EditorTable<TRow> table)
         {
            if (GUILayout.Button(HeaderText))
            {
               if (OnHeaderClickedSorter != null)
               {
                  if (m_reverseSort)
                  {
                     table.Order = new Comparison<TRow>((x, y) => -OnHeaderClickedSorter(x, y));
                     m_reverseSort = !m_reverseSort;
                  }
                  else
                  {
                     table.Order = OnHeaderClickedSorter;
                     m_reverseSort = !m_reverseSort;
                  }
               }
            }
         }

         public void DrawColumnCell(TRow row)
         {
            var temp = GUI.enabled;
            GUI.enabled = Enabled;

            DrawCell(row);

            GUI.enabled = temp;
         }

         protected abstract void DrawCell(TRow row);
      }

      public abstract class ColumnBaseGetterSetter<T> : ColumnBase
      {
         public Func<TRow, T> Getter { get; set; }
         public Action<TRow, T> Setter { get; set; }
      }

      public class ColumnUser : ColumnBase
      {
         public Action<TRow> Drawer { get; set; }

         protected override void DrawCell(TRow row)
         {
            Drawer?.Invoke(row);
         }
      }

      public class ColumnLabel : ColumnBase
      {
         public static GUIStyle Style { get; set; }

         public Func<TRow, string> Getter { get; set; }
         public Action<TRow> OnClicked { get; set; }

         public ColumnLabel()
         {
            Width = -1;
         }

         protected override void DrawCell(TRow row)
         {
            if (Style == null)
            {
               Style = new GUIStyle(GUI.skin.label);
               Style.wordWrap = true;
            }

            if (GUILayout.Button(Getter(row), Style))
            {
               OnClicked?.Invoke(row);
            }
         }
      }

      public class ColumnButton : ColumnBase
      {
         public string ButtonText { get; set; } = "X";
         public Action<TRow> OnClicked { get; set; }

         public ColumnButton()
         {
            Width = DefaultButtonWidth;
         }

         protected override void DrawCell(TRow row)
         {
            if (GUILayout.Button(ButtonText))
            {
               OnClicked?.Invoke(row);
            }
         }
      }

      public class ColumnRemoveButton : ColumnBase
      {
         public string ButtonText { get; set; } = "X";
         public Action<TRow> OnRemove { get; set; }

         public ColumnRemoveButton()
         {
            Width = DefaultButtonWidth;
         }

         protected override void DrawCell(TRow row)
         {
            if (GUILayout.Button(ButtonText))
            {
               OnRemove?.Invoke(row);
            }
         }
      }

      public class ColumnIntField : ColumnBaseGetterSetter<int>
      {
         protected override void DrawCell(TRow row)
         {
            EditorGUI.BeginChangeCheck();
            var v = EditorGUILayout.IntField(Getter(row));
            if (EditorGUI.EndChangeCheck())
            {
               Setter(row, v);
            }
         }
      }

      public class ColumnFloatField : ColumnBaseGetterSetter<float>
      {
         protected override void DrawCell(TRow row)
         {
            EditorGUI.BeginChangeCheck();
            var v = EditorGUILayout.FloatField(Getter(row));
            if (EditorGUI.EndChangeCheck())
            {
               Setter(row, v);
            }
         }
      }

      public class ColumnIntSlider : ColumnBaseGetterSetter<int>
      {
         public Vector2Int MinMax { get; set; }

         protected override void DrawCell(TRow row)
         {
            MinMax = new Vector2Int(Mathf.Min(MinMax.x, MinMax.y), Mathf.Max(MinMax.x, MinMax.y));

            EditorGUI.BeginChangeCheck();
            var v = EditorGUILayout.IntSlider(Getter(row), MinMax.x, MinMax.y);
            if (EditorGUI.EndChangeCheck())
            {
               Setter(row, v);
            }
         }
      }

      public class ColumnFloatSlider : ColumnBaseGetterSetter<float>
      {
         public Vector2 MinMax { get; set; }

         protected override void DrawCell(TRow row)
         {
            MinMax = new Vector2(Mathf.Min(MinMax.x, MinMax.y), Mathf.Max(MinMax.x, MinMax.y));

            EditorGUI.BeginChangeCheck();
            var v = EditorGUILayout.Slider(Getter(row), MinMax.x, MinMax.y);
            if (EditorGUI.EndChangeCheck())
            {
               Setter(row, v);
            }
         }
      }

      public class ColumnObjectField<T> : ColumnBaseGetterSetter<T> where T : UnityEngine.Object
      {
         public bool AllowSceneObject { get; set; }

         protected override void DrawCell(TRow row)
         {
            EditorGUI.BeginChangeCheck();
            var v = (T)EditorGUILayout.ObjectField(Getter(row), typeof(T), AllowSceneObject);
            if (EditorGUI.EndChangeCheck())
            {
               Setter(row, v);
            }
         }
      }

      public class ColumnTextField : ColumnBaseGetterSetter<string>
      {
         protected override void DrawCell(TRow row)
         {
            EditorGUI.BeginChangeCheck();
            var v = EditorGUILayout.TextField(Getter(row));
            if (EditorGUI.EndChangeCheck())
            {
               Setter(row, v);
            }
         }
      }
   }
}
#endif