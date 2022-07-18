#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace UnityCommon
{
    /// <summary>
    /// base class of editor drawing field
    /// </summary>
    public abstract class Field
    {
        public abstract void DrawField();

        /// <summary>
        /// Need call submit to apply this changes on drawing 
        /// </summary>
        public virtual void Submit() { }
    }

    public abstract class ValueField<T> : Field
    {
        public string Label { get; set; }
        public Action<T> Setter { get; set; }
        public Func<T> Getter { get; set; }

        protected T m_value;

        public ValueField(string label, Func<T> getter, Action<T> setter)
        {
            Label = label;
            Getter = getter;
            Setter = setter;
            m_value = getter();
        }

        public override void Submit()
        {
            Setter(m_value);
        }
    }

    public class CustomField : Field
    {
        public Action OnDraw { get; set; }
        public Action OnSubmit { get; set; }

        public override void DrawField()
        {
            OnDraw?.Invoke();
        }

        public override void Submit()
        {
            OnSubmit?.Invoke();
        }
    }

    public class Vector3IntField : ValueField<Vector3Int>
    {
        public Vector3IntField(string label, Func<Vector3Int> getter, Action<Vector3Int> setter) : base(label, getter, setter)
        {
        }

        public override void DrawField()
        {
            m_value = EditorGUILayout.Vector3IntField(Label, m_value);
        }
    }

    public class Vector3Field : ValueField<Vector3>
    {
        public Vector3Field(string label, Func<Vector3> getter, Action<Vector3> setter) : base(label, getter, setter)
        {
        }

        public override void DrawField()
        {
            m_value = EditorGUILayout.Vector3Field(Label, m_value);
        }
    }

    public class FloatField : ValueField<float>
    {
        public FloatField(string label, Func<float> getter, Action<float> setter) : base(label, getter, setter)
        {
        }

        public override void DrawField()
        {
            m_value = EditorGUILayout.FloatField(Label, m_value);
        }
    }

    public class FloatSliderField : ValueField<float>
    {
        public float Min { get; set; }
        public float Max { get; set; }

        public FloatSliderField(string label, float min, float max, Func<float> getter, Action<float> setter) : base(label, getter, setter)
        {
            Min = Mathf.Min(min, max);
            Max = Mathf.Max(min, max);
        }

        public override void DrawField()
        {
            m_value = EditorGUILayout.Slider(Label, m_value, Min, Max);
        }
    }

    public class IntField : ValueField<int>
    {
        public IntField(string label, Func<int> getter, Action<int> setter) : base(label, getter, setter)
        {
        }

        public override void DrawField()
        {
            m_value = EditorGUILayout.IntField(Label, m_value);
        }
    }

    public class IntSliderField : ValueField<int>
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public IntSliderField(string label, int min, int max, Func<int> getter, Action<int> setter) : base(label, getter, setter)
        {
            Min = Mathf.Min(min, max);
            Max = Mathf.Max(min, max);
        }

        public override void DrawField()
        {
            m_value = EditorGUILayout.IntSlider(Label, m_value, Min, Max);
        }
    }

    public class ToggleField : ValueField<bool>
    {
        public ToggleField(string label, Func<bool> getter, Action<bool> setter) : base(label, getter, setter)
        {
        }

        public override void DrawField()
        {
            m_value = EditorGUILayout.Toggle(Label, m_value);
        }
    }

    public class TextField : ValueField<string>
    {
        public TextField(string label, Func<string> getter, Action<string> setter) : base(label, getter, setter)
        {
        }

        public override void DrawField()
        {
            m_value = EditorGUILayout.TextField(Label, m_value);
            //m_value = GUILayout.TextField(Label, m_value);
        }
    }

    public class LabelField : Field
    {
        public string Label { get; set; }

        public LabelField(string label)
        {
            Label = label;
        }

        public override void DrawField()
        {
            EditorGUILayout.LabelField(Label);
        }
    }

    public class HeaderField : Field
    {
        public string Label { get; set; }
        public bool Bold { get; set; }
        public float Space { get; set; }

        public HeaderField(string label, bool bold = true, float space = 15)
        {
            Label = label;
            Bold = bold;
            Space = space;
        }

        public override void DrawField()
        {
            EditorGUILayout.LabelField(Label, Bold ? EditorStyles.boldLabel : EditorStyles.label);
        }
    }
}
#endif