using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   /// <summary>
   /// HP에 따라 스프라이트 전환
   /// </summary>
   class HPSpriteSwitcher : MonoBehaviour, IHPResponse
   {
      [SerializeField] List<SpriteAndCondition> m_steps;

      SpriteAndCondition m_current;

      private void OnValidate()
      {
         if (m_steps == null) m_steps = new List<SpriteAndCondition>();
         if (m_steps.Count <= 0) return;

         m_steps.Sort(new Comparison<SpriteAndCondition>((x, y) => { return x.Hp - y.Hp; }));

         for (int i = 0; i < m_steps.Count - 1; ++i)
         {
            if (m_steps[i].Hp == m_steps[i + 1].Hp)
            {
               m_steps[i + 1].Hp += 1;

               OnValidate();
               return;
            }
         }
      }

      public void OnHpChanged(int hp)
      {
         for (int i = 0; i < m_steps.Count; ++i)
         {
            var step = m_steps[i];
            if (step == m_current) continue;

            // switch sprite config
            if (step.Hp == hp)
            {
               m_current?.Configer.DisableConfig();
               step.Configer.EnableConfig();
               m_current = step;
            }
         }
      }

      [Serializable]
      class SpriteAndCondition
      {
         public SpriteRenderConfiger Configer;
         public int Hp;
      }
   }
}