using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   class LazyDisableGameObject : MonoBehaviour
   {
      public float sec = 1;

      private void OnEnable()
      {
         StartLazyDisable();
      }

      public async void StartLazyDisable()
      {
         await Task.Delay((int)(sec * 1000));

         gameObject.SetActive(false);
      }
   }
}