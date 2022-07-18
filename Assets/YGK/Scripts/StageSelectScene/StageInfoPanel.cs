using UnityEngine;
using UnityEngine.SceneManagement;

namespace YGK.StageSelectScene
{
   class StageInfoPanel : UIPanel
   {
      [SerializeField] RectTransform m_itemsHolder;
      [SerializeField] GoalUIItem m_itemPrefab;
      [SerializeField] TMPro.TMP_Text m_stageNameText;

      public override bool IsActive => gameObject.activeSelf;

      public override void Open()
      {
         gameObject.SetActive(true);

         ClearPrevItems();

         var stage = GlobalVariables.Instance.CurrentStage;

         // set subject ui
         foreach (var g in stage.GoalEntities)
         {
            var ge = g.GoalEntity;
            var gcnt = g.GoalCnt;

            GoalUIItem item = Instantiate(m_itemPrefab);
            item.SetConfig(ge.Image, gcnt);
            item.SetColor(g.GoalEntity.ColorName);
            item.transform.parent = m_itemsHolder;

            if (stage.GoalEntities.Count > 2)
            {
               item.transform.localScale = Vector3.one * 0.7f / (stage.GoalEntities.Count / 2f);
            }
            else
            {
               item.transform.localScale = Vector3.one * 0.7f;
            }
         }

         // set name ui
         m_stageNameText.text = "Level " + stage.name;
      }

      public override void Close()
      {
         gameObject.SetActive(false);
         ClearPrevItems();
      }

      void ClearPrevItems()
      {
         foreach (var item in GetComponentsInChildren<GoalUIItem>())
         {
            var itemTemp = item;
            UnityCommon.Execute.DestroyGameObject(ref itemTemp);
         }
      }

      public void ClickLoadButton()
      {
         Close();
         SceneManager.LoadScene("Stage");
      }
   }
}