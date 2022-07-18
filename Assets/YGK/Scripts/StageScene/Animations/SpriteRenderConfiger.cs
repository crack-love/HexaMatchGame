using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   /// <summary>
   /// 스프라이트 렌더 설정
   /// </summary>
   abstract class SpriteRenderConfiger : MonoBehaviour
   {
      public abstract void EnableConfig();
      public abstract void DisableConfig();
   }
}