using UnityEngine;
using UnityEngine.SceneManagement;
using YGK.Scenario.StageScene;

namespace YGK
{
   class PauseMenuPanel : MonoBehaviour
   {
      public void Show()
      {
         if (StateManager.Instance.CurrentState != StateType.Match)
         {
            return;
         }

         gameObject.SetActive(true);
      }

      public void Restart()
      {
         gameObject.SetActive(false);
         StateManager.Instance.Start();
      }

      public void Continue()
      {
         gameObject.SetActive(false);
         StateManager.Instance.ChangeState(StateType.Drop);
      }

      public void Quit()
      {
         SceneManager.LoadScene("StageSelect");
      }
   }
}