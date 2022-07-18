using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 2020.06.10.수.23.51
/// </summary>
namespace UnityCommon
{
    /// <summary>
    /// Binding field will be hidden
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class InspectorHelpSelfAttribute : PropertyAttribute
    {
        /// <summary>
        /// The help text to be displayed in the HelpBox
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The icon to be displayed in the HelpBox.
        /// </summary>
        public MessageType Type { get; }

        /// <summary>
        /// Reflection name of bool field or bool returning method
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// Adds a HelpBox to the Unity property inspector. (binding field will hided)
        /// </summary>
        /// <param name="below">Is Help displayed below of a Field</param>
        /// <param name="type">The icon to be displayed in the HelpBox.</param>
        public InspectorHelpSelfAttribute(string text, MessageType type = MessageType.None)
        {
            Text = text;
            Type = type;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InspectorHelpSelfAttribute))]
    class InspectorHelpSelfAttributeDrawer : PropertyDrawer
    {
        static readonly RectOffset padding = new RectOffset(8, 8, 8, 8);
        static readonly RectOffset margin = new RectOffset(5, 5, 5, 3);

        float helpHeight;

        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a HelpAttribute.
        /// </summary>
        public InspectorHelpSelfAttribute Attribute => (InspectorHelpSelfAttribute)attribute;
        
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            var helpAtt = Attribute;

            // Condition check
            if (!ConditionCheck(prop.serializedObject.targetObject, helpAtt.Condition))
            {
                return 0;
            }

            // Calculate the height of the HelpBox using the GUIStyle on the current skin
            // and the inspector window's currentViewWidth.
            var content = new GUIContent(helpAtt.Text);
            var style = GUI.skin.GetStyle("helpbox");

            // Set padding, margin
            // unity bug? : margin.vertical is ignored in calculation
            style.padding = padding;
            style.margin = margin;
            style.alignment = TextAnchor.MiddleLeft;

            // Get HelpAttribute Height
            helpHeight = style.CalcHeight(content, EditorGUIUtility.currentViewWidth - margin.horizontal);

            return helpHeight + margin.vertical;
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var helpAtt = Attribute;

            // Condition check
            if (!ConditionCheck(prop.serializedObject.targetObject, helpAtt.Condition))
            {
                return;
            }

            position.x = margin.left;
            position.y += margin.top;
            position.width = EditorGUIUtility.currentViewWidth - margin.horizontal;
            position.height = helpHeight;

            // Renders the HelpBox in the Unity inspector UI.
            EditorGUI.HelpBox(position, helpAtt.Text, helpAtt.Type);
        }

        bool ConditionCheck(object target, string name)
        {
            if (name == null) return true;

            Type classType = target.GetType();
            var field = classType.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var method = classType.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, binder: null, types: new Type[] { }, modifiers: null);
            
            if (field != null)
            {
                if (field.FieldType == typeof(bool))
                {
                    return (bool)field.GetValue(target);
                }
            }
            else if (method != null)
            {
                if (method.ReturnType == typeof(bool))
                {
                    return (bool)method.Invoke(target, null);
                }
            }

            throw new Exception("Finding Condition " + name + " Fail");
        }
    }
//#else
    // Replicate MessageType Enum if we are not in editor as this enum exists in UnityEditor namespace.
    // This should stop errors being logged the same as Shawn Featherly's commit in the Github repo but I
    // feel is cleaner than having the conditional directive in the middle of the HelpAttribute constructor.
  /*  public enum MessageType
    {
        None,
        Info,
        Warning,
        Error,
    }*/
#endif
}