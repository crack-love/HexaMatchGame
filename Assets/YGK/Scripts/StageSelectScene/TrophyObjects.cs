using UnityEngine;

namespace YGK.StageSelectScene
{
   class TrophyObjects : UnityCommon.MonoBehaviourSingletone<TrophyObjects>
   {
      public TrophyObject GetTrophyObject(StageEntity stage)
      {
         foreach(var child in GetComponentsInChildren<TrophyObject>())
         {
            if (child.StageEntity == stage)
            {
               return child;
            }
         }

         return null;
      }
   }
}