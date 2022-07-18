using UnityEngine;

namespace YGK
{
   class CameraSetter : MonoBehaviour
   {
      public float rowPadding = 0.5f;
      public float colPadding = 0.1f;
      public bool m_updateCameraWithValidation;

      private void OnValidate()
      {
         if (m_updateCameraWithValidation)
         {
            FitCamera();
         }
      }

      [UnityCommon.InspectorButton("FitCamera")] public bool btn0;
      public void FitCamera()
      {
         var cam = Camera.main;
         if (!cam) return;

         var map = BlockMap.Instance;
         if (!map) return;

         var rowSize = 0f;
         var colSize = 0f;
         rowSize = map.RowSize * map.RowGap * (1 + rowPadding) * cam.aspect;
         colSize = map.ColSize * map.ColGap * (1 + colPadding);
         cam.orthographicSize = Mathf.Max(rowSize, colSize);
      }
   }
}