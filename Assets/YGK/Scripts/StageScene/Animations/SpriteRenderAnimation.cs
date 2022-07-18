using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   /// <summary>
   /// 시간에 따른 스프라이트 변화 애니메이션
   /// </summary>
   class SpriteRenderAnimation : SpriteRenderConfiger
   {
      [SerializeField] SpriteRenderer m_renderer;
      [SerializeField] bool m_loop = false;
      [SerializeField] float m_duration = 1f;
      [SerializeField] Sprite[] m_sprites;
      bool m_running;

      public bool Running
      {
         get => m_running;
         set => m_running = value;
      }

      protected void Reset()
      {
         if (m_renderer == null)
         {
            m_renderer = GetComponentInChildren<SpriteRenderer>();
         }
      }

      public override void EnableConfig()
      {
          StartAnimationAsync().Watch();
      }

      public override void DisableConfig()
      {
         m_running = false;
      }

      public async Task StartAnimationAsync()
      {
         if (m_sprites.Length <= 0) return;

         float time = 0;
         m_running = true;

         while (this && enabled && m_running)
         {
            float p;
            if (m_duration <= 0)
               p = 0;
            else
               p = time / m_duration;

            if (p >= 1f)
            {
               if (m_loop)
               {
                  time = 0;
                  p = 0;
               }
               else
               {
                  p = 0.99f;
               }
            }

            int idx = (int)(m_sprites.Length * p);
            m_renderer.sprite = m_sprites[idx];

            // finish
            if (!m_loop && idx == m_sprites.Length - 1)
            {
               m_running = false;
               return;
            }

            await Task.Yield();
            time += Time.deltaTime;
         }
      }
   }
}