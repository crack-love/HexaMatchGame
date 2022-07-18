using UnityEngine;
using UnityEngine.UI;

namespace YGK
{
   static class ImageTool
   {
      /// <summary>
      /// 캔버스 스케일 역적용한 스크린 픽셀 사이즈
      /// </summary>
      /// <returns></returns>
      public static Vector2 GetTruePixelSize(Graphic image)
      {
         float scaleFactor = 1f;

         foreach (var canvas in image.GetComponentsInParent<Canvas>())
         {
            scaleFactor *= canvas.scaleFactor;
         }

         return image.rectTransform.rect.size * scaleFactor;
      }
   }
}