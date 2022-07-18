using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   class PopupStarterOnEnable : MonoBehaviour
   {
      [SerializeField] float delaySec = 0f;
      [SerializeField] float durSec = 0.3f;

      private async void OnEnable()
      {
         var endScale = transform.localScale;
         transform.localScale = Vector3.zero;

         await Task.Delay((int)(delaySec * 1000));

         await PopupAnimation.PopupSingle(transform, endScale, durSec);
      }
   }
}