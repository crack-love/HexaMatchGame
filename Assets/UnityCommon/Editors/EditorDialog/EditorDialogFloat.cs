#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace UnityCommon
{
    /// <summary>
    /// Use: Type.Show()
    /// </summary>
    public class EditorDialogFloat : EditorDialog
    {
        public float Value { get; set; }
        public string Label { get; set; }

        public static void Show(string label, float value, Action<EditorDialogFloat> submit, bool isModal = false)
        {
            var i = new EditorDialogFloat();
            i.Value = value;
            i.Label = label;
            i.SubmitButton.Callback = () => submit(i);
            i.IsModal = isModal;

            i.Show();
        }
        
        protected override void OnGUIContext()
        {
            Value = EditorGUILayout.FloatField(Label, Value);
        }
    }
}
#endif