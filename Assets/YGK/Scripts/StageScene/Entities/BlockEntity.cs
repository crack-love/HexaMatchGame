using UnityEngine;

namespace YGK
{
   // 블록
   // 매치블
   // 장애물
   // 아이템

   [CreateAssetMenu(fileName = "New Block", menuName = "Entity/Block")]
   class BlockEntity : ScriptableObject
   {
      [SerializeField] Block m_prefab = null;
      [SerializeField] int m_hp = 1;
      [SerializeField] bool m_canDrop = true;
      [SerializeField] bool m_canMatch = true;
      [SerializeField] bool m_canSwap = true;
      [SerializeField] bool m_canAffectOther = true;
      [SerializeField] bool m_canAffected = false;
      [SerializeField] bool m_canClick = false;
      [SerializeField] bool m_canMerge = false;
      [SerializeField] bool m_isGoal = false;
      [SerializeField] bool m_isItem = false;
      [SerializeField] ColorLayer m_colorLayer = ColorLayer.None;
      [SerializeField] string m_renderColorName;

      public string RenderColorName => m_renderColorName; // not using for check matching
      public Block Prefab => m_prefab;
      public int HP => m_hp;
      public bool CanDrop => m_canDrop;
      public bool CanMatch => m_canMatch;
      public bool CanSwap => m_canSwap;
      public bool CanAffected => m_canAffected;
      public bool CanAffectOther => m_canAffectOther;
      public bool CanClick => m_canClick;
      public bool CanMerge => m_canMerge;
      public bool IsGoal => m_isGoal;
      public bool IsItem => m_isItem;
      public ColorLayer ColorLayer => m_colorLayer;
   }
}