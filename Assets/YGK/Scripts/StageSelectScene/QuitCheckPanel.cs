using UnityEngine;
using UnityEngine.SceneManagement;

namespace YGK.StageSelectScene
{
   class QuitCheckPanel : UIPanel
   {
      public override bool IsActive => gameObject.activeSelf;

      public override void Open()
      {
         gameObject.SetActive(true);

      }

      public override void Close()
      {

         gameObject.SetActive(false);
      }

      public void ClickYesButton()
      {
         Application.Quit();
      }

      public void ClickNoButton()
      {
         Close();
      }
   }
}