using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YGK.Scenario.StageScene
{
   /*
    * 상태 전환은 이 매니저를 통해서 한다
    * 전환 조건 및 다음 상태는 각 상태가 관리한다
    * */

   public class StateManager : UnityCommon.MonoBehaviourSingletone<StateManager>
   {
      [SerializeField] List<Token> m_list;
      [SerializeField] State m_currState = null;

      StateType m_currentStateType;

      public StateType CurrentState => m_currentStateType;
      
      private void OnEnable()
      {
         if (m_list == null) m_list = new List<Token>();
      }

      public void Start()
      {
         // init all
         foreach (var token in m_list)
         {
            token.State.Initialize();
         }

         ChangeState(StateType.Start);
      }

      public void ChangeState(StateType state)
      {
         // disable current
         if (m_currState != null)
         {
            m_currState.enabled = false;
         }

         // enable next
         foreach(var t in m_list)
         {
            if (t.Type == state)
            {
               m_currentStateType = state;
               m_currState = t.State;
               t.State.enabled = true;
               t.State.BeginState();
               return;
            }
         }

         throw new System.Exception("State Change Fail to " + state);
      }

      [Serializable]
      struct Token
      {
         public StateType Type;
         public State State;
      }
   }
}