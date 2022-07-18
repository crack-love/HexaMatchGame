#if UNITY_EDITOR
using UnityEngine;
using UnityCommon;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;

/// <summary>
/// 2020-12-22 화 오후 7:16:33, 4.0.30319.42000, YONG-PC, Yong
/// </summary>
namespace UnityCommon
{
    public static class AssetDatabaseEx
    {
        public static readonly char[] PathSpliter = { System.IO.Path.AltDirectorySeparatorChar };

        /// <summary>
        /// dirPath:Assets/Dir0/Dir1
        /// </summary>
        public static void ValidateAssetDirPath(string dirPath)
        {
            var dirs = dirPath.Split(PathSpliter, StringSplitOptions.RemoveEmptyEntries);

            string parentPath = "";
            string currentPath = "";

            foreach (var dir in dirs)
            {
                if (parentPath.Length > 0)
                {
                    currentPath = parentPath + PathSpliter[0] + dir;
                }
                else
                {
                    currentPath = dir;
                }

                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    AssetDatabase.CreateFolder(parentPath, dir);
                }

                parentPath = currentPath;
            }
        }

        /// <summary>
        /// dirPath:Assets/Dir0/Dir1/File
        /// </summary>
        public static void ValidateAssetFilePathDir(string filePath)
        {
            var dirs = filePath.Split(PathSpliter, StringSplitOptions.RemoveEmptyEntries);

            string parentPath = "";
            string currentPath = "";

            for (int i = 0; i < dirs.Length - 1; ++i)
            {
                var dir = dirs[i];

                if (parentPath.Length > 0)
                {
                    currentPath = parentPath + PathSpliter[0] + dir;
                }
                else
                {
                    currentPath = dir;
                }

                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    AssetDatabase.CreateFolder(parentPath, dir);
                }

                parentPath = currentPath;
            }
        }

        /// <summary>
        /// dir : "Assets/.."
        /// </summary>
        public static List<T> LoadAllAssetsAtDir<T>(params string[] dirs) where T : Object
        {
            return LoadAllAssetsAtDir(new List<T>(), dirs);
        }

        /// <summary>
        /// dir : "Assets/.."
        /// </summary>
        public static List<T> LoadAllAssetsAtDir<T>(List<T> buffer, params string[] dirs) where T : Object
        {
            var type = typeof(T);
            var guids = AssetDatabase.FindAssets(null, dirs);

            if (buffer == null)
            {
                buffer = new List<T>(guids.Length);
            }
            else
            {
                buffer.Clear();
            }

            for (int i = 0; i < guids.Length; ++i)
            {
                var guid = guids[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(path, type);

                var adding = (T)asset;
                if (adding)
                {
                    buffer.Add(adding);
                }
            }

            return buffer;
        }
    }
}
#endif