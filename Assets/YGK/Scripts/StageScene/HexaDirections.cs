using System.Collections.Generic;
using UnityEngine;

namespace YGK
{
   static class HexaDirections
   {
      /// <summary>
      /// 드랍 가능한 다이렉션 모음
      /// </summary>
      public static readonly IReadOnlyList<HexaDirection> DownDirection = new HexaDirection[]
      {
         HexaDirection.Down,
         HexaDirection.LeftDown,
         HexaDirection.RightDown,
      };

      /// <summary>
      /// even's neighbor delta (짝수)
      /// </summary>
      public static readonly IReadOnlyList<(int row, int col)> EvenDelta = new (int, int)[6]
      {
         (1, 0), (0, -1), (0, +1), (-1, -1), (-1, +1), (-1, 0),
      };

      /// <summary>
      /// odd's neigbor delta (홀수)
      /// </summary>
      public static readonly IReadOnlyList<(int row, int col)> OddDelta = new (int, int)[6]
      {
         (1, 0), (+1, -1), (+1, +1), (0, -1), (0, +1), (-1, 0),
      };

      public static IReadOnlyList<(int row, int col)> GetDelta(int col)
      {
         return col % 2 == 0 ? EvenDelta : OddDelta;
      }
   }
}