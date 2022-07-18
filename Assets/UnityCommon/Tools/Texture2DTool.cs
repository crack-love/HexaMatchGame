using UnityEngine;
using UnityCommon;
using System;

/// <summary>
/// 2021-01-12 화 오후 5:19:38, 4.0.30319.42000, YONG-PC, Yong
/// </summary>
namespace UnityCommon
{
    public static class Texture2DTool
    {
        public static Texture2D CreateTexture(int sizeX, int sizeY, Color color)
        {
            var t = new Texture2D(sizeX, sizeY, TextureFormat.RGB24, false);
            var px = new Color[sizeX * sizeY];

            for (int i = 0; i < sizeX; ++i)
                for (int j = 0; j < sizeY; ++j)
                {
                    px[i + j * sizeX] = color;
                }

            t.SetPixels(px);
            t.Apply();

            return t;
        }
    }
}