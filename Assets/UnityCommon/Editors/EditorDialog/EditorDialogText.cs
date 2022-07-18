#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityCommon
{
    /// <summary>
    /// Use: Type.Show()
    /// </summary>
    public class EditorDialogText : EditorDialog
    {
        public string Label;
        public string Value;

        public static void Show(string label, string value, Action<EditorDialogText> submit)
        {
            var i = new EditorDialogText();
            i.Label = label;
            i.Value = value;
            i.OnSubmit = ()=>submit(i);

            i.Show();
        }

        protected override void OnGUIContext()
        {
            Value = EditorGUILayout.TextField(Label, Value);
        }
    }
}
#endif