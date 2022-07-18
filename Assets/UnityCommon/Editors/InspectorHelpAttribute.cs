//------------------------------------------------------------------------------------------------------------------
/// <log>
/// 2020.03.19. help 위치 위->아래 수정, 컨디션 추가, 각 필드/메소드/주석 정리, 패딩/마진 기능 fix, 멀티라인속성 지원 fix, 기타 fix
/// 2020.03.27. 네임스페이스 변경
/// 2020.06.10. after/below 옵션 추가
/// </log>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityCommon
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class InspectorHelpAttribute : PropertyAttribute
    {
        /// <summary>
        /// The help text to be displayed in the HelpBox
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Is Help displayed below of a Field
        /// </summary>
        public bool Below { get; }

        /// <summary>
        /// The icon to be displayed in the HelpBox.
        /// </summary>
        public MessageType Type { get; }

        /// <summary>
        /// Reflection name of bool field or bool returning method
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// Adds a HelpBox to the Unity property inspector this field.
        /// </summary>
        /// <param name="below">Is Help displayed below of a Field</param>
        /// <param name="type">The icon to be displayed in the HelpBox.</param>
        public InspectorHelpAttribute(string text, bool below = true, MessageType type = MessageType.None)
        {
            Text = text;
            Below = below;
            Type = type;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InspectorHelpAttribute))]
    class HelpAttributeDrawer : PropertyDrawer
    {
        // paddingHeight: Used for padding between the text and the HelpBox border.
        // marginHeight: Used to add some margin between the the HelpBox and the property.
        // minHeight: This stops icon shrinking if text content doesn't fill out the container enough.
        readonly RectOffset padding = new RectOffset(8, 8, 8, 8);
        readonly RectOffset margin = new RectOffset(5, 5, 2, 5);
        const int minHeight = 30;

        // Global field to store the original (base) property height.
        // addedHeight: Custom added height for drawing text area which has the MultilineAttribute.
        float baseHeight = 0;
        float helpHeight = 0;

        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a HelpAttribute.
        /// </summary>
        public InspectorHelpAttribute Attribute => (InspectorHelpAttribute)attribute;

        /// <summary>
        /// Get attribute this property have
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>return null if empty</returns>
        protected T TryGetPropertyAttribute<T>() where T : PropertyAttribute
        {
            var attributes = fieldInfo.GetCustomAttributes(typeof(T), true);
            return attributes != null && attributes.Length > 0 ? (T)attributes[0] : null;
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            var helpAttribute = Attribute;

            // We store the original property height for later use...
            baseHeight = base.GetPropertyHeight(prop, label);

            // Condition check
            if (!ConditionCheck(prop.serializedObject.targetObject, helpAttribute.Condition))
                return baseHeight;

            // Calculate the height of the HelpBox using the GUIStyle on the current skin and the inspector
            // window's currentViewWidth.
            var content = new GUIContent(helpAttribute.Text);
            var style = new GUIStyle(GUI.skin.GetStyle("helpbox"));

            // Set padding, margin (static registerd)
            style.padding = padding;
            style.margin = margin;
            style.alignment = TextAnchor.MiddleLeft;

            // Get HelpAttribute Height
            helpHeight = style.CalcHeight(content, EditorGUIUtility.currentViewWidth - margin.horizontal);

            // prevent icon shrinking
            /*
            if (helpAttribute.type != MessageType.None && helpHeight < minHeight)
            {
                RectOffset iconPadding = new RectOffset(padding.left, padding.right, padding.top + 5, padding.bottom + 5);
                style.padding = iconPadding;
                helpHeight = style.CalcHeight(content, EditorGUIUtility.currentViewWidth - margin.horizontal);
            }
            */

            // Since we draw a custom text area with the label above if our property contains the
            // MultilineAttribute, we need to add some extra height to compensate. This is stored in a
            // seperate global field so we can use it again later.
            var multilineAtt = TryGetPropertyAttribute<MultilineAttribute>();
            if (multilineAtt != null && prop.propertyType == SerializedPropertyType.String)
            {
                baseHeight += multilineAtt.lines * EditorGUIUtility.singleLineHeight;
            }

            return baseHeight + helpHeight + margin.vertical;
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var helpAttribute = Attribute;

            // Condition check
            if (!ConditionCheck(prop.serializedObject.targetObject, helpAttribute.Condition))
            {
                EditorGUI.PropertyField(position, prop);
                return;
            }

            // We get a local reference to the MultilineAttribute as we use it twice in this method and it
            // saves calling the logic twice for minimal optimization, etc...
            var multiline = TryGetPropertyAttribute<MultilineAttribute>();
            var range = TryGetPropertyAttribute<RangeAttribute>();

            EditorGUI.BeginProperty(position, label, prop);

            // Copy the position out so we can calculate the position of our HelpBox without affecting the
            // original position.
            var basePos = position;
            var helpPos = position;

            if (helpAttribute.Below)
            {
                basePos.height = baseHeight;
                helpPos.y += basePos.height + margin.top;
                helpPos.x = margin.left;
                helpPos.width = EditorGUIUtility.currentViewWidth - margin.horizontal;
                helpPos.height = helpHeight;
            }
            else
            {
                helpPos.height = helpHeight;
                helpPos.y += margin.top;
                helpPos.x = margin.left;
                helpPos.width = EditorGUIUtility.currentViewWidth - margin.horizontal;

                basePos.height = baseHeight;
                basePos.y += helpPos.height + margin.bottom;
            }

            if (helpAttribute.Below)
            {
                //Debug.Log("Below");
                // Renders the base Property
                DrawBaseProperty(basePos, prop, label, range, multiline);

                // Renders the HelpBox in the Unity inspector UI.
                EditorGUI.HelpBox(helpPos, helpAttribute.Text, helpAttribute.Type);
            }
            else
            {
                //Debug.Log("!Below");
                EditorGUI.HelpBox(helpPos, helpAttribute.Text, helpAttribute.Type);

                DrawBaseProperty(basePos, prop, label, range, multiline);
            }

            EditorGUI.EndProperty();
        }

        void DrawBaseProperty(Rect position, SerializedProperty prop, GUIContent label, RangeAttribute range, MultilineAttribute multiline)
        {
            // If we have a RangeAttribute on our field, we need to handle the PropertyDrawer differently to
            // keep the same style as Unity's default.
            if (range != null)
            {
                if (prop.propertyType == SerializedPropertyType.Float)
                {
                    EditorGUI.Slider(position, prop, range.min, range.max, label);
                }
                else if (prop.propertyType == SerializedPropertyType.Integer)
                {
                    EditorGUI.IntSlider(position, prop, (int)range.min, (int)range.max, label);
                }
                else
                {
                    // Not numeric so draw standard property field as punishment for adding RangeAttribute to
                    // a property which can not have a range :P
                    EditorGUI.PropertyField(position, prop, label);
                }
            }
            else if (multiline != null)
            {
                // Here's where we handle the PropertyDrawer differently if we have a MultiLineAttribute, to try
                // and keep some kind of multiline text area. This is not identical to Unity's default but is
                // better than nothing...
                if (prop.propertyType == SerializedPropertyType.String)
                {
                    var posLabel = position;
                    var posTextArea = position;

                    posLabel.height = GUI.skin.label.CalcHeight(label, EditorGUIUtility.currentViewWidth);
                    posTextArea.height = position.height - posLabel.height;
                    posTextArea.y = position.y + posLabel.height;

                    EditorGUI.LabelField(posLabel, label);
                    prop.stringValue = EditorGUI.TextArea(posTextArea, prop.stringValue);
                }
                else
                {
                    // Again with a MultilineAttribute on a non-text field deserves for the standard property field
                    // to be drawn as punishment :P
                    EditorGUI.PropertyField(position, prop, label);
                }
            }
            else
            {
                // If we get to here it means we're drawing the default property field below the HelpBox. More custom
                // and built in PropertyDrawers could be implemented to enable HelpBox but it could easily make for
                // hefty else/if block which would need refactoring!
                EditorGUI.PropertyField(position, prop, label);
            }
        }

        bool ConditionCheck(object target, string checkerReflectionName)
        {
            if (checkerReflectionName == null) return true;

            Type classType = target.GetType();
            var field = classType.GetField(checkerReflectionName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var method = classType.GetMethod(checkerReflectionName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, binder: null, types: new Type[] { }, modifiers: null);
            Debug.Log(field);
            Debug.Log(method);
            if (field != null)
            {
                Debug.Log(field.FieldType + "/" + typeof(bool));
                if (field.FieldType == typeof(bool))
                {
                    return (bool)field.GetValue(target);
                }
            }
            else if (method != null)
            {
                Debug.Log(method.ReturnType + "/" + typeof(bool));
                if (method.ReturnType == typeof(bool))
                {
                    return (bool)method.Invoke(target, null);
                }
            }

            throw new Exception("Finding Condition " + checkerReflectionName + " Fail");
        }

    }
    // MessageType exists in UnityEditor namespace and can throw an exception when used outside the editor.
    // We spoof MessageType at the bottom of this script to ensure that errors are not thrown when
    // MessageType is unavailable.
#else
    // Replicate MessageType Enum if we are not in editor as this enum exists in UnityEditor namespace.
    // This should stop errors being logged the same as Shawn Featherly's commit in the Github repo but I
    // feel is cleaner than having the conditional directive in the middle of the HelpAttribute constructor.
    public enum MessageType
    {
        None,
        Info,
        Warning,
        Error,
    }
#endif
}