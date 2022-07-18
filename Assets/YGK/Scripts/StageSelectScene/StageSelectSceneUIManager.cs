using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YGK
{
   class StageSelectSceneUIManager : UnityCommon.MonoBehaviourSingletone<StageSelectSceneUIManager>
   {
      [SerializeField] UIPanel m_stageInfoPanel;
      [SerializeField] UIPanel m_quitCheckPanel;

      Stack<UIPanel> m_stack = new Stack<UIPanel>();

      public bool IsUIOpend => m_stack.Count > 0;

      public void OpenStageInfoPanel()
      {
         m_stack.Push(m_stageInfoPanel);
         m_stageInfoPanel.Open();
      }

      public void OpenQuitPanel()
      {
         m_stack.Push(m_quitCheckPanel);
         m_quitCheckPanel.Open();
      }

      void Update()
      {
         // update stack
         while (m_stack.Count > 0)
         {
            if (m_stack.Peek().IsActive)
               break;
            else
               m_stack.Pop();
         }

         if (Input.GetKeyDown(KeyCode.Escape))
         {
            if (m_stack.Count <= 0)
            {
               OpenQuitPanel();

               m_stack.Push(m_quitCheckPanel);
            }
            else
            {
               var top = m_stack.Pop();

               top.Close();
            }
         }
      }
   }
}