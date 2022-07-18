#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class SceneViewRaycaster
{
   /// <summary>
   /// SceneView.beforeSceneGui
   /// </summary>
   public static event Action<SceneView> BeforeSceneGUI
   {
      add => SceneView.beforeSceneGui += value;
      remove => SceneView.beforeSceneGui -= value;
   }

   /// <summary>
   /// SceneView.duringSceneGui
   /// </summary>
   public static event Action<SceneView> DuringSceneGUI
   {
      add => SceneView.duringSceneGui += value;
      remove => SceneView.duringSceneGui -= value;
   }

   public static bool ScenViewMousePositionRaycast<T>(out RaycastHit hit, out T hitComponent, int layermask = ~0) where T : class
    {
        ScenViewMousePositionRaycast(out hit, layermask);
        
        hitComponent = hit.transform?.GetComponent<T>();

        if (hitComponent != null)
        {
            return true;
        }
        
        return false;
    }

    public static bool ScenViewMousePositionRaycast(out RaycastHit hit, int layermask = ~0)
    {
        if (Event.current == null)
        {
            hit = new RaycastHit();
            return false;
        }

        var mousePos = Event.current.mousePosition;
        var sv = SceneView.currentDrawingSceneView;
        var ppp = EditorGUIUtility.pixelsPerPoint;

        // mousePos to screenPoint
        mousePos.y = sv.camera.pixelHeight - mousePos.y * ppp;
        mousePos.x *= ppp;
      
        // raycast
        Ray ray = sv.camera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit, 10000f, layermask))
        {
            return true;
        }

        return false;
    }

   public static Vector3 ScreenPointToWorld(SceneView view)
   {
      var mousePos = Event.current.mousePosition;
      var sv = view;
      var ppp = EditorGUIUtility.pixelsPerPoint;
      
      // mousePos to screenPoint
      mousePos.y = sv.camera.pixelHeight - mousePos.y * ppp;
      mousePos.x *= ppp;

      Vector3 res = Vector3.zero;
      try
      { 
         res = sv.camera.ScreenToWorldPoint(mousePos);
      }
#pragma warning disable CS0168 // e' 변수가 선언되었지만 사용되지 않았습니다.
      catch(Exception e)
#pragma warning restore CS0168 // e' 변수가 선언되었지만 사용되지 않았습니다.
      {
         // ignore out of view frustrum exception
         return Vector3.zero;
      }

      return res;
   }
}
#endif