#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityCommon
{
    /// <summary>
    /// 버튼 한 개 메시지 다이얼로그
    /// Use: Type.Show()
    /// </summary>
    public class EditorDialogMessage : EditorDialog
    {
        public string Message;

        public static void Show(string message)
        {
            var dialog = new EditorDialogMessage();
            dialog.Message = message;
            dialog.Show();
        }

        public EditorDialogMessage() : base("Message")
        {
            CancelButton.Hidden = true;
            SubmitButton.Text = "OK";
        }

        protected override void OnGUIContext()
        {
            EditorGUILayout.LabelField(Message);
        }
    }
}
#endif