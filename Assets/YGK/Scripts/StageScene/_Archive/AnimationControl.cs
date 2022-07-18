using UnityEngine;

namespace YGK
{
   [RequireComponent(typeof(Animation))]
   class AnimationControl : MonoBehaviour
   {
      [SerializeField] Animation m_animation;

      private void Reset()
      {
         m_animation = gameObject.AddComponent<Animation>();
         //m_animation.hideFlags = HideFlags.HideInInspector;
      }

      private void OnValidate()
      {
      }
   }

}