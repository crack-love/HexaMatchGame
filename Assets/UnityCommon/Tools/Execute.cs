using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityCommon
{
   /// <summary>
   /// Branch execution between Runtime and Editor
   /// </summary>
   public static class Execute
   {
#if UNITY_EDITOR
      public static void DestroyAsset(ref Object src)
      {
         if (!src) return;

         AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(src));
         src = null;
      }
#endif

      public static void DestoryScript(ref Object src)
      {
         DestoryScript(ref src, 0);
      }

      public static void DestoryScript(ref Object src, float sec)
      {
         if (!src) return;

         if (Application.isEditor)
         {
            // todo:lazy destruction
            Object.DestroyImmediate(src);
         }
         else
         {
            if (sec <= 0)
               Object.Destroy(src);
            else
               Object.Destroy(src, sec);
         }

         src = null;
      }

      public static void DestroyGameObject(ref GameObject src)
      {
         if (!src) return;

         if (Application.isEditor)
         {
            Object.DestroyImmediate(src);
         }
         else
         {
            Object.Destroy(src);
         }

         src = null;
      }

      public static void DestroyGameObject<T>(ref T src) where T : Component
      {
         DestroyGameObject(ref src, 0);
      }

      public static void DestroyGameObject<T>(ref T src, float sec) where T : Component
      {
         if (!src) return;

         if (Application.isEditor)
         {
            // todo:lazy destruction
            Object.DestroyImmediate(src.gameObject);
         }
         else
         {
            if (sec <= 0)
            {
               Object.Destroy(src.gameObject);
            }
            else
               Object.Destroy(src.gameObject, sec);
         }

         src = null;
      }

      public static void SetDirty(Object src)
      {
#if UNITY_EDITOR
         // make scene.unity or .asset dirty
         EditorUtility.SetDirty(src);

         // additinal set dirty prefab override
         if (PrefabUtility.IsPartOfAnyPrefab(src))
         {
            PrefabUtility.RecordPrefabInstancePropertyModifications(src);
         }
#endif
      }

      public static void DontDestroyOnLoad(Object o)
      {
         if (Application.isPlaying)
         {
            Object.DontDestroyOnLoad(o);
         }
      }
   }
}