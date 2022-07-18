using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   abstract class ItemBehaviour : MonoBehaviour
   {
      public abstract Task StartAnimation();

      public abstract void CollectRange(Block src, List<Block> collected);
   }
}