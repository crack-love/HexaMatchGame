using UnityEngine;

namespace YGK.StageSelectScene
{
   class StageSelectSceneVariables : UnityCommon.MonoBehaviourSingletone<StageSelectSceneVariables>
   {
      public bool IsDragging;
      public float PopupAnimationSec = 0.3f;
   }
}