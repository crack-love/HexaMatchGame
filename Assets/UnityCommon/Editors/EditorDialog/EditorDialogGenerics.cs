#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace UnityCommon
{
    public class EditorDialogGenerics : EditorDialog
    {
        List<Field> m_fields = new List<Field>();
        
        public static void Show(Field field, params Field[] fields)
        {
            EditorDialogGenerics res = new EditorDialogGenerics();

            res.AddField(field);

            if (fields != null)
            {
                for (int i = 0; i < fields.Length; ++i)
                {
                    res.AddField(fields[i]);
                }
            }

            res.Show();
        }

        public void AddField(Field field)
        {
            m_fields.Add(field);
        }

        public void AddVector3Int(string label, Func<Vector3Int> getter, Action<Vector3Int> setter)
        {
            AddField(new Vector3IntField(label, getter, setter));
        }

        public void AddVector3(string label, Func<Vector3> getter, Action<Vector3> setter)
        {
            AddField(new Vector3Field(label, getter, setter));
        }

        public void AddFloat(string label, Func<float> getter, Action<float> setter)
        {
            AddField(new FloatField(label, getter, setter));
        }

        public void AddInt(string label, Func<int> getter, Action<int> setter)
        {
            AddField(new IntField(label, getter, setter));
        }

        public void AddIntSlider(string label, int min, int max, Func<int> getter, Action<int> setter)
        {
            AddField(new IntSliderField(label, min, max, getter, setter));
        }

        public void AddFloatSlider(string label, float min, float max, Func<float> getter, Action<float> setter)
        {
            AddField(new FloatSliderField(label, min, max, getter, setter));
        }

        public void AddText(string label, Func<string> getter, Action<string> setter)
        {
            AddField(new TextField(label, getter, setter));
        }

        public void AddToggle(string label, Func<bool> getter, Action<bool> setter)
        {
            AddField(new ToggleField(label, getter, setter));
        }

        public void AddLabel(string label)
        {
            AddField(new LabelField(label));
        }

        public void AddHeader(string label, bool isBold = true, float space = 15)
        {
            AddField(new HeaderField(label, isBold, space));
        }

        protected override void OnGUIContext()
        {
            foreach(var field in m_fields)
            {
                field.DrawField();
            }
        }

        protected override void OnPreSubmit()
        {
            foreach (var field in m_fields)
            {
                field.Submit();
            }
        }
    }
}
#endif