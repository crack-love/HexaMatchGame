using UnityEngine;

namespace YGK.Scenario.StageScene
{
   abstract class State : MonoBehaviour
   {
      public abstract void Initialize();

      public abstract void BeginState();
   }
}