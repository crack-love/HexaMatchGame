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
   /// 싱글톤 제공
   /// </summary>
   public class ObjectSingletone<TDerived> where TDerived : class, new()
   {
      protected static object m_mutex;
      protected static TDerived m_instance;

      static ObjectSingletone()
      {
         m_mutex = new object();
      }

      public static TDerived Instance
      {
         get
         {
            if (m_instance == null)
            {
               lock (m_mutex)
               {
                  if (m_instance == null)
                  {
                     m_instance = new TDerived();
                  }
               }
            }

            return m_instance;
         }
      }
   }
}