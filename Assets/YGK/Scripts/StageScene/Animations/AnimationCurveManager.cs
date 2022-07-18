using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace YGK
{
   class AnimationCurveManager : UnityCommon.MonoBehaviourSingletone<AnimationCurveManager>
   {
      [Serializable]
      struct CurveObeject
      {
         public string name;
         public AnimationCurve curve;
      }

      [SerializeField] List<CurveObeject> m_list;

      OrderedDictionary m_dic;
      
      private void OnEnable()
      {
         m_dic = new OrderedDictionary();
         foreach (CurveObeject o in m_list)
         {
            m_dic.Add(o.name, o.curve);
         }
      }

      public AnimationCurve GetCurve(string name)
      {
         return m_dic[name] as AnimationCurve;
      }
   }
}