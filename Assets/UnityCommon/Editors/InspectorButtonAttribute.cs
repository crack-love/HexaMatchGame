//------------------------------------------------------------------------------------------------------------------
/// <log>
/// 2020.03.27 : 네임스페이스 변경. 주석 정리
/// </log>
//------------------------------------------------------------------------------------------------------------------
using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityCommon
{
    /// <summary>
    /// 메소드 호출 버튼. 필드에 바인딩됨. 객체/정적/공개/비공개 메소드 모두 가능 <para />
    /// 사용법 : [InspectorButton("Method")] public bool tempField; void Method() {}
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public readonly string      label;
        public readonly string      methodName;
        public readonly float       buttonWidth;
        public readonly string[]    pars;

        /// <param name="MethodName">버튼 클릭시 호출할 메소드 이름</param>
        /// <param name="label">버튼 텍스트</param>
        /// <param name="Width">버튼 가로 길이</param>
        /// <param name="Pars">메소드에 인자로 전달되는 필드 이름 (여러개 가능)</param>
		public InspectorButtonAttribute(string MethodName, string label=null, int Width=200, params string[] Pars)
        {
			methodName = MethodName;
			this.label = label != null ? label : MethodName;
            buttonWidth = Width;
            pars = Pars.Length <= 0 ? null : Pars;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    class ButtonAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// 버튼 높이 반환
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GUI.skin.button.CalcHeight(label, (attribute as InspectorButtonAttribute).buttonWidth);
        }

        /// <summary>
        /// 버튼 드로우. 리플렉션 이용 메소드 실행
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            // 기존 프로퍼티 드로우
            // EditorGUILayout.PropertyField(prop);

            // 초기화
            var att = attribute as InspectorButtonAttribute;
			var labelText = att.label;
            var methodName = att.methodName;
            var btnWidth = att.buttonWidth;
            var pars = att.pars;

            // 버튼 위치
            var viewWidth = EditorGUIUtility.currentViewWidth;
            var x = viewWidth / 2 - btnWidth / 2;
            var y = position.y;
            var btnHeight = position.height;
            Rect buttonRect = new Rect(x, y, btnWidth, btnHeight);
            
            // 버튼 드로우
            if (GUI.Button(buttonRect, labelText))
            {
                // 리플렉션 초기화
                object target = prop.serializedObject.targetObject;
                Type classType = target.GetType();
                Type[] parTypes = new Type[0];
                object[] parValues = new object[0];
                MethodInfo methodInfo = null;
                
                // Get Pars
                if (pars != null)
                {
                    parTypes = new Type[pars.Length];
                    parValues = new object[pars.Length];

                    for (int i = 0; i < pars.Length; ++i)
                    {
                        var field = classType.GetField(pars[i],
                            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        parTypes[i] = field.FieldType;
                        parValues[i] = field.GetValue(target);
                    }
                }

                // Get Method
                methodInfo = classType.GetMethod(methodName,
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, 
                    null, parTypes, null);
                
                if (methodInfo != null)
                {
                    methodInfo.Invoke(target, parValues);
                }
                else
                {
                    string partypes = "{ ";
                    string parvalues = "{ ";

                    foreach (var v in parTypes)
                        partypes += v.ToString() + " ";

                    foreach (var v in parValues)
                        parvalues += v.ToString() + " ";

                    partypes += "}";
                    parvalues += "}";

                    Debug.LogError(string.Format("InspectorButton: Unable to find method name={0} in type={1}, parTypes={2}, parValues={3}", methodName, classType, partypes, parvalues));
                }
            }
        }
    }
    #endif
}