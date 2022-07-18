using UnityCommon;
using UnityEngine;

namespace YGK
{
   class GlobalVariables : UnityCommon.MonoBehaviourSingletone<GlobalVariables>
   {
      public StageEntity CurrentStage;

      static int m_priorID = 0;

      private void OnEnable()
      {
         if (m_priorID == 0)
         {
            m_priorID = GetInstanceID();
            DontDestroyOnLoad(transform.parent.gameObject);
         }
         else if (m_priorID == GetInstanceID())
         {

         }
         else
         {
            if (Application.isPlaying)
            {
               Destroy(transform.parent.gameObject);
            }
            else
            {
               DestroyImmediate(transform.parent.gameObject);
            }
         }
      }
   }
}