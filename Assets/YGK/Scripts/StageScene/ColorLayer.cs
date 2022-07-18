using UnityEngine;

namespace YGK
{
   [System.Serializable]
   [System.Flags]
   public enum ColorLayer
   {
      None = 0b0,
      Green = 0b1,
      Yellow = 0b10,
      Blue = 0b100,
      Purple = 0b1000,
      Red = 0b10000,
      White = 0b100000,

   }
}