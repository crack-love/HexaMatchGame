#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 2021-02-03 수 오후 6:37:27, 4.0.30319.42000, YONG-PC, Yong
/// </summary>
namespace UnityCommon
{
   /// <summary>
   /// Derived from ScriptableObject (Serializable)
   /// </summary>
   public class Toolbar : ScriptableObject
   {
      // structure copy for shortcut convinience
      public enum ButtonSize
      {
         Fixed = GUI.ToolbarButtonSize.Fixed,
         FitToContents = GUI.ToolbarButtonSize.FitToContents,
      }

      [SerializeField] ButtonSize m_buttonSize;
      [SerializeField] List<ToolbarItem> m_items;
      // itemNames must be string[] type for param gui
      [SerializeField] string[] m_itemNames;
      [SerializeField] int m_selected;
      [SerializeField] ScriptableObject m_parent;
      [SerializeField] bool m_enabled;
      [NonSerialized] bool m_isInitialized;

      public ScriptableObject Parent
      {
         get => m_parent;
         set => m_parent = value;
      }

      public ButtonSize ButtonsSize
      {
         get => m_buttonSize;
         set => m_buttonSize = value;
      }

      public bool Enabled => m_enabled;
      
      public void OnEnable()
      {
         if (m_items == null)
         {
            m_items = new List<ToolbarItem>();
         }
      }

      public void OnDisable()
      {
         if (0 <= m_selected && m_selected < m_items.Count)
         {
            m_items[m_selected].OnDeselected();
         }

         if (!m_parent)
            DestroyImmediate(this);
      }

      public void SetEnable(bool value)
      {
         if (m_enabled && !value)
         {
            OnDisable();
         }
         else if (!m_enabled && value)
         {
            OnEnable();
         }

         m_enabled = value;
      }

      void Validate()
      {
         if (m_items.Count <= 0) return;
         
         // clamp idx
         m_selected = Mathf.Clamp(m_selected, 0, m_items.Count - 1);

         // update names
         if (m_itemNames == null || m_itemNames.Length != m_items.Count)
         {
            m_itemNames = new string[m_items.Count];
         }
         for (int i = 0; i < m_items.Count; ++i)
         {
            m_itemNames[i] = m_items[i].name;
         }

         // select current showing item
         if (!m_isInitialized)
         {
            m_isInitialized = true;
            m_items[m_selected].OnSelected();
         }
      }
      
      public T Add<T>() where T : ToolbarItem
      {
         var res = CreateInstance<T>();
         res.SetToolbar(this);

         m_items.Add(res);

         return res;
      }

      public void OnGUI()
      {
         Validate();

         EditorGUI.BeginChangeCheck();

         // toolbar gui
         var newSelected = GUILayout.Toolbar(m_selected, m_itemNames, null, (GUI.ToolbarButtonSize)m_buttonSize);

         // change selection
         if (EditorGUI.EndChangeCheck() && newSelected != m_selected)
         {
            var oldOne = m_items[m_selected];
            var newOne = m_items[newSelected];
            oldOne.IsSelected = false;
            newOne.IsSelected = true;
            oldOne.OnDeselected();
            newOne.OnSelected();
            m_selected = newSelected;
         }

         // selected gui
         m_items[m_selected].OnGUI();
      }
   }
}

#endif