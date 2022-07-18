using UnityEngine;

namespace YGK
{
   // bind to camera to cache datas?
   static class CameraTool
   {
      public static Vector3 ScreenToWorldFactor(Camera cam)
      {
         var worldSize = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
         worldSize.x /= (float)cam.pixelWidth;
         worldSize.y /= (float)cam.pixelHeight;
         worldSize.z /= (float)cam.nearClipPlane;

         //worldSize.Scale(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane).Reciprocal());
         return worldSize;
      }

      public static Vector3 WorldBotCenterPoint(Camera cam)
      {
         var spos = new Vector3(cam.pixelWidth / 2f, 0f, cam.nearClipPlane);
         return cam.ScreenToWorldPoint(spos);
      }

      public static Vector3 WorldTopCenterPoint(Camera cam)
      {
         var spos = new Vector3(cam.pixelWidth / 2f, cam.pixelHeight, cam.nearClipPlane);
         return cam.ScreenToWorldPoint(spos);
      }

      public static Vector3 WorldLeftCenterPoint(Camera cam)
      {
         var spos = new Vector3(0f, cam.pixelHeight / 2f, cam.nearClipPlane);
         return cam.ScreenToWorldPoint(spos);
      }

      public static Vector3 WorldRightCenterPoint(Camera cam)
      {
         var spos = new Vector3(cam.pixelWidth, cam.pixelHeight / 2f, cam.nearClipPlane);
         return cam.ScreenToWorldPoint(spos);
      }

      public static Vector3 WorldCenterPoint(Camera cam)
      {
         var spos = new Vector3(cam.pixelWidth / 2f, cam.pixelHeight / 2f, cam.nearClipPlane);
         return cam.ScreenToWorldPoint(spos);
      }

      public static Vector3 WorldPoint(ScreenPoint src, Camera cam)
      {
         switch (src)
         {
            case ScreenPoint.Center:
               return WorldCenterPoint(cam);
            case ScreenPoint.TopCenter:
               return WorldTopCenterPoint(cam);
            case ScreenPoint.BotCenter:
               return WorldBotCenterPoint(cam);
            case ScreenPoint.LeftCenter:
               return WorldLeftCenterPoint(cam);
            case ScreenPoint.RightCenter:
               return WorldRightCenterPoint(cam);
         }

         return default;
      }
   }
}