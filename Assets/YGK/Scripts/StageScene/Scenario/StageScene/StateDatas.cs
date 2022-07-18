using System.Collections.Generic;
using UnityEngine;
using YGK.Scenario.StageScene;

namespace YGK
{
   class StateDatas : UnityCommon.ObjectSingletone<StateDatas>
   {
      // POP STATE DATAS ///////////////////////////////////////////

      /// <summary>
      /// 엘리멘트 기준으로 팝 모양 검사
      /// </summary>
      public HashSet<Block> DropPopChecks = new HashSet<Block>();

      /// <summary>
      /// 엘리멘트 기준으로 팝 모양 검사
      /// </summary>
      public HashSet<Block> SwapPopChecks = new HashSet<Block>();
      
      /// <summary>
      /// 무조건 팝
      /// </summary>
      public HashSet<Block> AbsolutePopBlocks = new HashSet<Block>();

      /// <summary>
      /// 아이템 사용 애니메이션 각 개체간 비동기 수행 (파티)
      /// </summary>
      public bool AsyncronouseItemAnimation = false;
   }
}