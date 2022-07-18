#if UNITY_EDITOR
using UnityEngine;
using UnityCommon;
using System;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// 2020-12-22
/// </summary>
namespace UnityCommon
{
    /// <summary>
    /// This is addtional option of <see cref="GUIStyle"/> can find at <see cref="GUI.skin"/> or <see cref="EditorStyles"/>
    /// </summary>
    public abstract class GUIStyleOption
    {
        internal abstract GUIStyle Apply(GUIStyle src);

        public static GUIStyleOption Padding(RectOffset padding)
        {
            return new PaddingOption(padding);
        }

        public static GUIStyleOption Padding(int padding)
        {
            return new PaddingOption(padding, padding, padding, padding);
        }

        public static GUIStyleOption Padding(int left, int right, int top, int bottom)
        {
            return new PaddingOption(left, right, top, bottom);
        }

        public static GUIStyleOption Margin(RectOffset margin)
        {
            return new MarginOption(margin);
        }

        public static GUIStyleOption Margin(int margin)
        {
            return new MarginOption(margin, margin, margin, margin);
        }

        public static GUIStyleOption Margin(int left, int right, int top, int bottom)
        {
            return new MarginOption(left, right, top, bottom);
        }

        public static GUIStyleOption TextAlign(TextAnchor align)
        {
            return new TextAlignOption(align);
        }

        public static GUIStyleOption BackgroundColor(Color color)
        {
            return new BackgroundOption(color);
        }

        public static GUIStyleOption BackgroundColor(float r, float g, float b)
        {
            return new BackgroundOption(r, g, b);
        }

        public static GUIStyleOption TextColor(Color color)
        {
            return new TextColorOption(color);
        }

        public static GUIStyleOption TextColor(float r, float g, float b)
        {
            return new TextColorOption(new Color(r, g, b));
        }

        public static GUIStyleOption Border(RectOffset border)
        {
            return new BorderOption(border);
        }

        public static GUIStyleOption Border(int border)
        {
            return new BorderOption(new RectOffset(border, border, border, border));
        }

        public static GUIStyleOption Border(int left, int right, int top, int bottom)
        {
            return new BorderOption(new RectOffset(left, right, top, bottom));
        }

        public static GUIStyleOption Clone(string style)
        {
            return new CloneOption(style);
        }

        /// <summary>
        /// if not fixed width = 0
        /// </summary>
        public static GUIStyleOption Width(float width)
        {
            return new WidthOption(width);
        }

        public static GUIStyleOption TextBold()
        {
            return new TextBoldOption();
        }

        // ----------------------------------------------------------------------
        // Option Classes
        // ----------------------------------------------------------------------

        class TextAlignOption : GUIStyleOption
        {
            TextAnchor m_alignment;

            public TextAlignOption(TextAnchor align)
            {
                m_alignment = align;
            }

            internal override GUIStyle Apply(GUIStyle src)
            {
                src.alignment = m_alignment;
                return src;
            }
        }

        class MarginOption : GUIStyleOption
        {
            Vector4Int m_margin;

            public MarginOption(RectOffset offset)
            {
                if (offset == null)
                {
                    m_margin = default;
                }
                else
                {
                    m_margin.x = offset.left;
                    m_margin.y = offset.right;
                    m_margin.z = offset.top;
                    m_margin.w = offset.bottom;
                }
            }

            public MarginOption(int left, int right, int top, int bottom)
            {
                m_margin.x = left;
                m_margin.y = right;
                m_margin.z = top;
                m_margin.w = bottom;
            }

            internal override GUIStyle Apply(GUIStyle src)
            {
                src.margin.left = m_margin.x;
                src.margin.right = m_margin.y;
                src.margin.top = m_margin.z;
                src.margin.bottom = m_margin.w;

                return src;
            }
        }

        class PaddingOption : GUIStyleOption
        {
            Vector4Int m_padding;

            public PaddingOption(RectOffset offset)
            {
                if (offset == null)
                {
                    m_padding = default;
                }
                else
                {
                    m_padding.x = offset.left;
                    m_padding.y = offset.right;
                    m_padding.z = offset.top;
                    m_padding.w = offset.bottom;
                }
            }

            public PaddingOption(int left, int right, int top, int bottom)
            {
                m_padding.x = left;
                m_padding.y = right;
                m_padding.z = top;
                m_padding.w = bottom;
            }

            internal override GUIStyle Apply(GUIStyle src)
            {
                src.padding.left = m_padding.x;
                src.padding.right = m_padding.y;
                src.padding.top = m_padding.z;
                src.padding.bottom = m_padding.w;

                return src;
            }
        }

        class BackgroundOption : GUIStyleOption
        {
            Texture2D m_tex;

            public BackgroundOption(Color color)
            {
                m_tex = Texture2D(color);
            }

            public BackgroundOption(float r, float g, float b)
            {
                m_tex = Texture2D(r, g, b);
            }

            internal override GUIStyle Apply(GUIStyle src)
            {
                if (src != null)
                {
                    if (src.normal != null)
                    {
                        src.normal.background = m_tex;
                    }
                }

                return src;
            }

            Texture2D Texture2D(Color c)
            {
                var res = new Texture2D(1, 1);
                res.SetPixel(0, 0, c);
                res.Apply();
                return res;
            }

            Texture2D Texture2D(float r, float g, float b)
            {
                var res = new Texture2D(1, 1);
                res.SetPixel(0, 0, new Color(r, g, b));
                res.Apply();
                return res;
            }
        }

        class TextColorOption : GUIStyleOption
        {
            Color m_color;

            public TextColorOption(Color color)
            {
                m_color = color;
            }

            internal override GUIStyle Apply(GUIStyle src)
            {
                src.normal.textColor = m_color;

                return src;
            }
        }

        class CloneOption : GUIStyleOption
        {
            string m_name;

            public CloneOption(string name)
            {
                m_name = name;
            }

            internal override GUIStyle Apply(GUIStyle src)
            {
                var found = new GUIStyle(GUI.skin.FindStyle(m_name));

                if (found != null)
                {
                    return found;
                }
                else
                {
                    return src;
                }
            }
        }

        class BorderOption : GUIStyleOption
        {
            RectOffset m_border;

            public BorderOption(RectOffset border)
            {
                m_border = border;
            }

            internal override GUIStyle Apply(GUIStyle src)
            {
                src.border = m_border;

                return src;
            }
        }

        class WidthOption : GUIStyleOption
        {
            float m_width;

            /// <summary>
            /// if non fixed width = 0
            /// </summary>
            public WidthOption(float width)
            {
                m_width = width;
            }

            internal override GUIStyle Apply(GUIStyle src)
            {
                src.fixedWidth = m_width;
                src.stretchWidth = Mathf.Approximately(m_width, 0) ? true : false;

                return src;
            }
        }

        class TextBoldOption : GUIStyleOption
        {
            internal override GUIStyle Apply(GUIStyle src)
            {
                src.fontStyle = FontStyle.Bold;

                return src;
            }
        }
    }
}
#endif