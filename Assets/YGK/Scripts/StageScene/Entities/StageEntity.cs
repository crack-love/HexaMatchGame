using System;
using System.Collections.Generic;
using UnityEngine;

namespace YGK
{
   /// <summary>
   /// Initial Stage Settings
   /// </summary>
   [CreateAssetMenu(fileName = "New Stage", menuName = "Entity/Stage")]
   partial class StageEntity : ScriptableObject
   {
      [SerializeField] int m_moveCnt;
      [SerializeField] int[] m_scoreHuddles;
      [SerializeField] Texture2D m_backgroundTexture;
      [SerializeField] Texture2D m_startTexture;
      [SerializeField] List<BlockEntity> m_dropBlocks;
      [SerializeField] List<GoalEntityAndCnt> m_goalEntities;
      [SerializeField] StageEntity m_nextStage;

      public int MoveCnt => m_moveCnt;
      public List<BlockEntity> DropBlocks => m_dropBlocks;
      public Texture2D BackgroundTexture => m_backgroundTexture;
      public Texture2D StartTexture => m_startTexture;
      public List<GoalEntityAndCnt> GoalEntities => m_goalEntities;
      public StageEntity NextStage => m_nextStage;

      public int GetScoreHuddle(int i)
      {
         return m_scoreHuddles[i];
      }
   }

   [Serializable]
   struct GoalEntityAndCnt
   {
      public GoalEntity GoalEntity;
      public int GoalCnt;
   }
}