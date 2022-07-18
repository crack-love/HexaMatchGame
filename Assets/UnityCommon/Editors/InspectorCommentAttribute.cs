//------------------------------------------------------------------------------------------------------------------
/// <log>
/// 2020.04.04 : 생성
/// </log>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityCommon
{
    /// <summary>
    /// 에디터 인스펙터상에서 작성하는 멀티라인 코멘트
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class InspectorCommentAttribute : PropertyAttribute
    {
        public RectOffset Padding = new RectOffset(8, 8, 8, 8);
        public RectOffset Margin = new RectOffset(5, 5, 2, 5);
        public Color TextColor = Color.black;
        public Color BackColor = Color.grey;

        public InspectorCommentAttribute()
        {

        }

        public InspectorCommentAttribute(Color textColor, Color backColor)
        {
            TextColor = textColor;
            BackColor = backColor;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InspectorCommentAttribute))]
    class CommentAttributeDrawer : PropertyDrawer
    {
        new InspectorCommentAttribute attribute;
        GUIStyle style;

        public CommentAttributeDrawer()
        {
            attribute = (InspectorCommentAttribute)base.attribute;
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            attribute = (InspectorCommentAttribute)base.attribute;
            var textContent = new GUIContent(prop.stringValue);

            if (style == null)
            {
                style = new GUIStyle(GUI.skin.textArea);
                style.padding = attribute.Padding;
                style.margin = attribute.Margin;
                style.alignment = TextAnchor.UpperLeft;

                // colors
                style.normal.textColor = attribute.TextColor;
                var tex = new Texture2D(1, 1); // background
                tex.SetPixel(0, 0, attribute.BackColor);
                style.normal.background = tex;
            }            

            var height = style.CalcHeight(textContent, EditorGUIUtility.currentViewWidth - style.margin.horizontal);

            return height + style.margin.vertical;
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            OnGUINoColor(position, prop, label);
        }

        void OnGUINoColor(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);

            // Render
            prop.stringValue = EditorGUI.TextArea(position, prop.stringValue, style);

            EditorGUI.EndProperty();
        }

    }
#endif
}