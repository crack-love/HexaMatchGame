using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   static class PopupAnimation
   {
      public static async Task PopupAll<T>(IEnumerable<T> list, float durSec) where T : MonoBehaviour
      {
         List<Task> tasks = new List<Task>();

         foreach (var src in list)
         {
            tasks.Add(PopupSingle(src.transform, durSec));
         }

         await Task.WhenAll(tasks);
      }

      public static async Task PopupSingle(Transform transform, Vector3 endLocalScale, float durSec)
      {
         float t = 0;

         while (t < durSec)
         {
            var p = t / durSec;
            p = Mathf.Clamp01(p);

            var scale = Vector3.Lerp(Vector3.zero, endLocalScale, p);

            if (!transform) return;
            transform.localScale = scale;

            await Task.Yield();
            t += Time.deltaTime;
         }

         transform.localScale = endLocalScale;
      }

      public static Task PopupSingle(Transform transform, float durSec)
      {
         return PopupSingle(transform, transform.localScale, durSec);
      }
   }
}