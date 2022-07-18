using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace YGK.Scenario.StageScene
{
   // TODO : set Camera width addated from stage cols size, rows size
   class StartState : State
   {
      /*
       * 환경 초기화
       * 그림 애니메이션
       * 플레이 시작 (드랍 모드로)
       * */

      [SerializeField] CameraSetter m_camSetter;
      [SerializeField] Graphic m_startImage;
      [SerializeField] Transform m_startImagePanel;
      [SerializeField] float m_startTextureShowDur = 1.5f;

      public override void Initialize()
      {
         // map set
         Stage.Instance.LoadStage(GlobalVariables.Instance.CurrentStage);
         BlockMap.Instance.gameObject.SetActive(false);

         // camera set
         m_camSetter.FitCamera();
      }

      public override async void BeginState()
      {
         // image animation
         await ShowImageAnimation(m_startImage, m_startImagePanel, ScreenPoint.RightCenter, ScreenPoint.LeftCenter);

         // pupup map
         BlockMap.Instance.gameObject.SetActive(true);
         var pt0 = PopupAnimation.PopupAll(BlockMap.Instance.GetBlockEnumerable(), 0.3f);
         var pt1 = PopupAnimation.PopupAll(BlockMap.Instance.GetSocketEnumerable(), 0.5f);
         await Task.WhenAll(pt0, pt1);
         
         StateManager.Instance.ChangeState(StateType.Drop);
      }

      async Task ShowImageAnimation(Graphic src, Transform srcHolder, ScreenPoint begPos, ScreenPoint endPos)
      {
         var cam = Camera.main;
         float approachSec = 0.5f;
         float fallbackSec = 0.3f;
         var screenToWorldFactor = CameraTool.ScreenToWorldFactor(cam);
         var imageSize = ImageTool.GetTruePixelSize(src);
         imageSize.Scale(screenToWorldFactor);

         srcHolder.gameObject.SetActive(true);

         var beg = CameraTool.WorldPoint(begPos, cam) + Vector3.right * imageSize.x * 1.2f;
         var end = CameraTool.WorldCenterPoint(cam);

         var approachAni = new Simation(beg, end, srcHolder, approachSec);
         approachAni.Start();

         // wait input or dur
         await InputTool.WaitInputToClick(m_startTextureShowDur + approachSec);
         if (!approachAni.AsyncTask.IsCompleted)
            approachAni.Cancel();

         end = CameraTool.WorldPoint(endPos, cam) + Vector3.left * imageSize.x * 1.2f;
         await new Simation(end, srcHolder, fallbackSec).StartAsync();
      }
   }

}