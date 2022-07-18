using UnityEngine;

namespace YGK
{
   class GoalUIItem : MonoBehaviour
   {
      [SerializeField] TMPro.TMP_Text m_cntText;
      [SerializeField] UnityEngine.UI.RawImage m_image;

      public void SetConfig(Texture2D tex, int cnt)
      {
         m_cntText.text = cnt.ToString();
         m_image.texture = tex;
      }

      public void SetCount(int cnt)
      {
         m_cntText.text= cnt.ToString();
      }

      public void SetColor(string name)
      {
         if (name != null && name.Length > 0)
            m_image.color = ColorManager.Instance.GetColorByString(name);
      }
   }
}