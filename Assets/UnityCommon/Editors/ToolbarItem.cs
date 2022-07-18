#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// 2021-06-25 금 오후 3:47:58, 4.0.30319.42000, YONG-PC, Yong
/// </summary>
namespace UnityCommon
{
   public abstract class ToolbarItem : ScriptableObject
   {
      [SerializeField] Toolbar m_toolbar;
      [SerializeField] bool m_isSelected;

      // overriding Object.name field
      public abstract new string name { get; }

      public bool IsSelected
      {
         get => m_isSelected;
         internal set => m_isSelected = value;
      }

      public void SetToolbar(Toolbar src)
      {
         m_toolbar = src;
      }

      public Toolbar Toolbar
      {
         get => m_toolbar;
      }

      public abstract void OnGUI();

      public virtual void OnSelected()
      {
      }

      public virtual void OnDeselected()
      {
      }
   }
}
#endif