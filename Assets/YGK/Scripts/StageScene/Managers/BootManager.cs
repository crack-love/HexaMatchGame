#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityCommon;
using System.Collections.Generic;
using System;

/// <summary>
/// 2021-06-08 화 오후 4:39:19, 4.0.30319.42000, YONG-PC, Yong
/// </summary>
namespace YGK
{
   [ExecuteAlways]
   partial class BootManager : MonoBehaviourSingletone<BootManager>
   {
      [Serializable]
      class ManagerToken
      {
         public MonoBehaviour Mono;
         public string LastName; // debug
         public int StartOrder;
         public int FinalOrder;
      }

      [SerializeField] List<ManagerToken> m_managers;

      void Reset()
      {
         if (m_managers == null) m_managers = new List<ManagerToken>();
         foreach (var cmp in GetComponentsInChildren<IManager>())
         {
            VerifyRegistration(cmp);
         }
      }

      private void OnEnable()
      {
         if (m_managers == null) m_managers = new List<ManagerToken>();

         Validation();

         SortAsStart(true);

         // OnStartManager
         foreach (var token in m_managers)
         {
            var man = (IManager)token.Mono;

            man.OnStartManager();
         }
      }

      private void OnDisable()
      {
         Validation();

         SortAsFinal(true);

         // OnFinalManager
         foreach (var token in m_managers)
         {
            var man = (IManager)token.Mono;

            man.OnFinalManager();
         }
      }

      /// <summary>
      /// check is registed param manager
      /// </summary>
      public void VerifyRegistration(IManager src)
      {
         var mono = (MonoBehaviour)src;

         for (int i = 0; i < m_managers.Count; ++i)
         {
            if (m_managers[i].Mono == mono)
            {
               return;
            }
         }

         m_managers.Add(new ManagerToken()
         {
            Mono = mono,
         });

         // set param to child
         mono.transform.parent = transform;
         mono.name = src.GetType().Name;

#if UNITY_EDITOR
         // ping
         EditorUtility.SetDirty(this);
         EditorGUIUtility.PingObject(mono);
#endif
      }

      /// <summary>
      /// Check managers are valid
      /// </summary>
      void Validation()
      {
         for (int i = 0; i < m_managers.Count; ++i)
         {
            if (!m_managers[i].Mono)
            {
               Debug.Log("Invalid Manager Removed : " + m_managers[i].LastName);
               m_managers.RemoveAt(i);
               --i;
            }

            // record name for debug
            m_managers[i].LastName = m_managers[i].Mono.name;
         }
      }

      void SortAsName(bool isAscent)
      {
         if (isAscent)
         {
            m_managers.Sort((x, y) =>
            {
               return string.Compare(x.Mono.name, y.Mono.name);
            });
         }
         else
         {
            m_managers.Sort((x, y) =>
            {
               return string.Compare(y.Mono.name, x.Mono.name);
            });
         }
      }

      void SortAsStart(bool isAscent)
      {
         if (isAscent)
         {
            m_managers.Sort((x, y) =>
            {
               return x.StartOrder - y.StartOrder;
            });
         }
         else
         {
            m_managers.Sort((x, y) =>
            {
               return y.StartOrder - x.StartOrder;
            });
         }
      }

      void SortAsFinal(bool isAscent)
      {
         if (isAscent)
         {
            m_managers.Sort((x, y) =>
            {
               return x.FinalOrder - y.FinalOrder;
            });
         }
         else
         {
            m_managers.Sort((x, y) =>
            {
               return y.FinalOrder - x.FinalOrder;
            });
         }
      }

      class StartComparer : IComparer<ManagerToken>
      {
         public int Compare(ManagerToken x, ManagerToken y)
         {
            return x.StartOrder - y.StartOrder;
         }
      }

      class FinalComparer : IComparer<ManagerToken>
      {
         public int Compare(ManagerToken x, ManagerToken y)
         {
            return x.FinalOrder - y.FinalOrder;
         }
      }
   }
}