using UnityEngine;

namespace YGK
{
   abstract class GridObject : MonoBehaviour
   {
      [SerializeField] int m_xidx;
      [SerializeField] int m_yidx;
      [SerializeField] int m_hp;

      public int Row => m_xidx;
      public int Col => m_yidx;
      public int HP => m_hp;

      public void SetIndex(int x, int y)
      {
         m_xidx = x;
         m_yidx = y;
      }

      public void SetHP(int value)
      {
         if (value < 0)
            value = 0;

         if (value != m_hp)
         {
            m_hp = value;

            foreach (var cmp in GetComponents<IHPResponse>())
            {
               cmp.OnHpChanged(m_hp);
            }
         }
      }

      private void OnDrawGizmos()
      {
#if UNITY_EDITOR
         UnityEditor.Handles.Label(transform.position, "" + m_xidx + "/" + m_yidx);
#endif
      }
   }
}