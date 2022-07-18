using UnityEngine;

namespace YGK.StageSelectScene
{
   class StageSelectSceneCamMover : MonoBehaviour
   {
      [SerializeField] Camera m_cam;
      [SerializeField] float m_minY = 0;
      [SerializeField] float m_maxY = 10;
      [SerializeField] float m_speed = 2;
      [SerializeField] float m_dragStart = 0.01f;
      [SerializeField] bool m_invertY;

      bool m_dragging = false;
      Vector3 m_firstSpos;
      Vector3 m_lastSpos;
      Vector3 m_screenToWorldAspect;

      private void OnEnable()
      {
         m_screenToWorldAspect = m_cam.WorldToScreenPoint(transform.position + Vector3.one);
         m_screenToWorldAspect = m_screenToWorldAspect.Reciprocal();
      }

      private void Update()
      {
         if (Input.GetMouseButton(0))
         {
            if (Input.GetMouseButtonDown(0))
            {
               m_firstSpos = Input.mousePosition;
            }

            // check drag start
            if (!m_dragging)
            {
               var spos = Input.mousePosition;
               var sdelta = m_firstSpos - spos;
               var wdelta = sdelta;
               wdelta.Scale(m_screenToWorldAspect);

               if (wdelta.sqrMagnitude > m_dragStart)
               {
                  m_lastSpos = spos;
                  m_dragging = true;
               }
            }

            // drag
            if (m_dragging)
            {
               var spos = Input.mousePosition;
               var sdelta = m_lastSpos - spos;
               var wdelta = sdelta;
               wdelta.Scale(m_screenToWorldAspect);

               var newCamPos = m_cam.transform.position + Vector3.up * wdelta.y * m_speed * (m_invertY ? -1 : 1);
               newCamPos.y = Mathf.Clamp(newCamPos.y, m_minY, m_maxY);
               m_cam.transform.position = newCamPos;

               m_lastSpos = spos;
               m_dragging = true;
            }
            
            StageSelectSceneVariables.Instance.IsDragging = m_dragging;
         }
         else
         {
            if (Input.GetMouseButtonUp(0))
            {
               m_dragging = false;
            }
         }
      }
   }
}