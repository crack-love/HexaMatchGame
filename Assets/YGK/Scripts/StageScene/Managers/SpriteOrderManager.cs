using System.Collections.Generic;
using UnityEngine;

namespace YGK
{
   class SpriteOrderManager : Manager<SpriteOrderManager>
   {
      [SerializeField] NameAndOrder[] m_list;

      Dictionary<string, int> m_dic;
      
      public override void OnStartManager()
      {
         m_dic = new Dictionary<string, int>();
         foreach (NameAndOrder data in m_list)
         {
            m_dic.Add(data.Name, data.Order);
         }
      }

      public int GetOrder(string name)
      {
         return m_dic[name];
      }

      [System.Serializable]
      class NameAndOrder
      {
         public string Name;
         public int Order;
      }
   }
}