using UnityEngine;
using UnityEngine.SceneManagement;
using YGK.Scenario.StageScene;

namespace YGK
{
   class ClearMenuPanel : MonoBehaviour
   {
      [SerializeField] TMPro.TMP_Text m_levelNameText;
      [SerializeField] TMPro.TMP_Text m_scoreText;

      public void Show()
      {
         m_levelNameText.text = "Level " + Stage.Instance.Entity.name;
         m_scoreText.text = Stage.Instance.Score.ToString("#,###");
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

      public void Next()
      {
         SceneManager.LoadScene("StageSelect");
      }
   }
}