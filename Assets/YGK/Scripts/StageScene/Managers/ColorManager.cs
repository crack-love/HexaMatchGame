using System.Collections.Generic;
using UnityEngine;

namespace YGK
{
   [ExecuteAlways]
   class ColorManager : UnityCommon.MonoBehaviourSingletone<ColorManager>
   {
      [SerializeField] Token[] m_colors;
      [SerializeField] TokenStr[] m_colorStrs;

      Dictionary<ColorLayer, Color> m_dic;
      Dictionary<string, Color> m_dicStr;

      private void OnEnable()
      {
         m_dic = new Dictionary<ColorLayer, Color>();
         foreach (Token t in m_colors)
         {
            m_dic.Add(t.Layer, t.Color);
         }

         m_dicStr = new Dictionary<string, Color>();
         foreach (TokenStr t in m_colorStrs)
         {
            m_dicStr.Add(t.Name, t.Color);
         }
      }

      private void OnDisable()
      {
         
      }

      public Color GetColorByString(string name)
      {
         return m_dicStr[name];
      }

      public Color GetColor(ColorLayer layer)
      {
         if (layer == ColorLayer.None)
         {
            // 일반적으로 none은 컬러 설정이 없음을 의미하므로
            // 여기에 도달해서는 안된다

            return Color.white;
         }

         return m_dic[layer];
      }

      [System.Serializable]
      struct Token
      {
         public ColorLayer Layer;
         public Color Color;
      }

      [System.Serializable]
      struct TokenStr
      {
         public string Name;
         public Color Color;
      }
   }
}