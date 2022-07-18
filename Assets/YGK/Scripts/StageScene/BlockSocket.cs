using UnityEngine;

namespace YGK
{
   class BlockSocket : GridObject
   {
      [SerializeField] SocketEntity m_entity;

      public SocketEntity Entity => m_entity;

      public void SetEntity(SocketEntity e)
      {
         m_entity = e;
         SetHP(e.HP);
      }

      public void SetColor(string name)
      {
         if (name != null && name.Length > 0)
         {
            GetComponentInChildren<SpriteRenderer>().color = ColorManager.Instance.GetColorByString(name);
         }
      }
   }
}