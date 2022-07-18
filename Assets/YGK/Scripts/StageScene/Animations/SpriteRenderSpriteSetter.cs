using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   class SpriteRenderSpriteSetter : SpriteRenderConfiger
   {
      [SerializeField] SpriteRenderer m_renderer;
      [SerializeField] Sprite m_sprites;
      [SerializeField] ColorLayer m_color;

      protected void Reset()
      {
         if (m_renderer == null)
         {
            m_renderer = GetComponentInChildren<SpriteRenderer>();
         }
      }

      public override void EnableConfig()
      {
         m_renderer.sprite = m_sprites;
         if (m_color != ColorLayer.None)
         {
            m_renderer.color = ColorManager.Instance.GetColor(m_color);
         }
      }

      public override void DisableConfig()
      {

      }
   }
}