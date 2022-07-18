using System;
using System.Collections.Generic;
using UnityEngine;
using UnityCommon;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

namespace YGK
{
   [ExecuteAlways]
   class Stage : MonoBehaviourSingletone<Stage>
   {
      [SerializeField] StageEntity m_entity;
      [SerializeField] int m_remainMove = 0;
      [SerializeField] int m_score = 0;

      // ui
      [SerializeField] TextMeshProUGUI m_scoreCntText;
      [SerializeField] TextMeshProUGUI m_moveCntText;
      [SerializeField] RectTransform m_goalImageTransform;
      [SerializeField] RawImage m_backgroundImage;
      [SerializeField] RawImage m_startImage;
      [SerializeField] GoalUIItem m_goalUiItemPrefab;
      [SerializeField] RectTransform m_goalUiItemHolder;

      List<GoalRemain> m_reaminGoals = new List<GoalRemain>();

      public StageEntity Entity => m_entity;
      public int RemainMove => m_remainMove;
      public int Score => m_score;
      public RectTransform GoalImageTransform => m_goalImageTransform;
      
      private void Update()
      {
         if (Application.isEditor &&
            m_entity != null &&
            m_entity.IsEditorSceneNeedUpdate)
         {
            m_entity.IsEditorSceneNeedUpdate = false;

            LoadStage(m_entity);

            FindObjectOfType<CameraSetter>().FitCamera();            
         }
      }

      [Serializable]
      class GoalRemain
      {
         public GoalEntity Entity;
         public GoalUIItem UIItem;
         public int Count;
      }

      public bool ContainGoalEntity(GoalEntity e)
      {
         foreach(var token in m_reaminGoals)
         {
            if (token.Entity == e)
               return true;
         }

         return false;
      }

      void CreateGoalItems()
      {
         // clear prev ui element
         foreach(var item in m_goalUiItemHolder.GetComponentsInChildren<GoalUIItem>())
         {
            var copy = item;
            if (copy)
            {
               Execute.DestroyGameObject(ref copy);
            }
         }
         m_reaminGoals.Clear();

         // create ui elements and set remain goals
         foreach (var token in m_entity.GoalEntities)
         {
            var uiitem = Instantiate(m_goalUiItemPrefab);
            var cnt = token.GoalCnt;
            var ge = token.GoalEntity;

            uiitem.SetConfig(ge.Image, cnt);
            uiitem.transform.parent = m_goalUiItemHolder;
            if (m_entity.GoalEntities.Count > 2)
            {
               uiitem.transform.localScale = Vector3.one / (m_entity.GoalEntities.Count / 2f);
            }
            else
            {
               uiitem.transform.localScale = Vector3.one;
            }
            uiitem.SetColor(token.GoalEntity.ColorName);

            var adding = new GoalRemain()
            {
               UIItem = uiitem,
               Count = cnt,
               Entity = ge,
            };

            m_reaminGoals.Add(adding);
         }
      }

      public int GetRemainGoalCnt()
      {
         int res = 0;
         foreach(var token in m_reaminGoals)
         {
            res += token.Count;
         }
         return res;
   }

      /// <summary>
      /// Not load BlockMap
      /// </summary>
      /// <param name="e"></param>
      public void LoadStage(StageEntity e)
      {
         m_entity = e;

         // set fields
         m_remainMove = e.MoveCnt;
         m_score = 0;

         // goal items
         CreateGoalItems();

         // update UI
         AddRemainMoveDelta(0);
         AddScore(0);
         m_backgroundImage.texture = e.BackgroundTexture;
         m_startImage.texture = e.StartTexture;

         var map = BlockMap.Instance;
         map.SetEntity(m_entity);
      }

      public BlockEntity GetNextDropBlock()
      {
         var dropBlocks = m_entity.DropBlocks;

         var idx = UnityEngine.Random.Range(0, dropBlocks.Count);
         return dropBlocks[idx];
      }

      public void UpdateGoalUIs()
      {
         foreach(var token in m_reaminGoals)
         {
            token.UIItem.SetCount(token.Count);
         }
      }

      public void AddRemainGoalDelta(int delta, bool updateUI, GoalEntity src)
      {
         for (int i = 0; i < m_reaminGoals.Count; ++ i)
         {
            var token = m_reaminGoals[i];
            if (token.Entity != src) continue;

            token.Count += delta;
            if (token.Count < 0) token.Count = 0;

            if (updateUI)
            {
               token.UIItem.SetCount(token.Count);
            }
         }
      }

      public void AddRemainMoveDelta(int delta)
      {
         m_remainMove += delta;
         if (m_remainMove < 0) m_remainMove = 0;
         m_moveCntText.text = m_remainMove.ToString();
      }

      public void AddScore(int score)
      {
         m_score += score;
         if (m_score <= 0)
         {
            m_score = 0;
            m_scoreCntText.text = "0";
         }
         else
         {
            m_scoreCntText.text = m_score.ToString("#,###");
         }
      }
   }
}