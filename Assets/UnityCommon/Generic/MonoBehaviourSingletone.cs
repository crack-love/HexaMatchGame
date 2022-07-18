#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// 2021-01-27 수 오후 4:18:44, 4.0.30319.42000, YONG-PC, Yong
/// </summary>
namespace UnityCommon
{
   /// <summary>
   /// 싱글톤 제공 MonoBehaviour 베이스
   /// </summary>
   /// <typeparam name="TDerived"></typeparam>
   public class MonoBehaviourSingletone<TDerived> : MonoBehaviour where TDerived : MonoBehaviour
   {
      protected static object m_mutex;
      protected static TDerived m_instance;

      static MonoBehaviourSingletone()
      {
         m_mutex = new object();
      }

      public static TDerived Instance
      {
         get
         {
            if (!m_instance)
            {
               lock (m_mutex)
               {
                  if (!m_instance)
                  {
                     FindOrCreate();
                  }
               }
            }

            return m_instance;
         }
      }

      static void FindOrCreate()
      {
         // find
         m_instance = FindObjectOfType<TDerived>() as TDerived;

         // create
         if (!m_instance)
         {
            GameObject o = new GameObject(typeof(TDerived).Name);
            m_instance = o.AddComponent<TDerived>();
         }
      }
   }
}