using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YGK.StageSelectScene
{
   class TrophyObject : MonoBehaviour
   {
      [SerializeField] StageEntity m_stageEntity;
      [SerializeField] TMPro.TMP_Text m_textUI;

      public StageEntity StageEntity => m_stageEntity;

      private void OnValidate()
      {
         m_textUI.text = m_stageEntity.name;
      }

      /// <summary>
      /// Click
      /// </summary>
      public void OnMouseUp()
      {
         if (StageSelectSceneVariables.Instance.IsDragging) return;
         if (StageSelectSceneUIManager.Instance.IsUIOpend) return;

         // set stage
         GlobalVariables.Instance.CurrentStage = m_stageEntity;

         // active panel
         StageSelectSceneUIManager.Instance.OpenStageInfoPanel();
      }

      private bool IsPointerOverUIObject()
      {
         PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
         eventDataCurrentPosition.position = Input.mousePosition;
         List<RaycastResult> results = new List<RaycastResult>();
         EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
         
         return results.Count > 0;
      }
   }
}