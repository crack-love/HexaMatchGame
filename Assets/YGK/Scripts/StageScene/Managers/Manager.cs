using UnityEngine;
using UnityCommon;

/// <summary>
/// 2021-07-06 화 오후 7:52:37, 4.0.30319.42000, YONG-PC, Yong
/// </summary>
namespace YGK
{
   public abstract class Manager<TDerived> : MonoBehaviourSingletone<TDerived>, IManager
     where TDerived : Manager<TDerived>
   {
      private void Reset()
      {
         BootManager.Instance.VerifyRegistration(this);
      }

      public virtual void OnStartManager() { }
      public virtual void OnFinalManager() { }
   }

   interface IManager
   {
      void OnStartManager();
      void OnFinalManager();
   }
}