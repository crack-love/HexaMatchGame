#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace UnityCommon
{
    /// <summary>
    /// Use: Type.Show()
    /// </summary>
    public class EditorDialogVector3Int : EditorDialog
    {
        public Vector3Int Value;
        public string Label;

        public static void Show(string label, Vector3Int value, Action<EditorDialogVector3Int> submit)
        {
            var i = new EditorDialogVector3Int();
            i.Value = value;
            i.Label = label;
            i.OnSubmit = ()=>submit(i);

            i.Show();
        }

        protected override void OnGUIContext()
        {
            Value = EditorGUILayout.Vector3IntField(Label, Value);
        }
    }
}
#endif