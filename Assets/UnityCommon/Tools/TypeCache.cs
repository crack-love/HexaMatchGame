using UnityEngine;
using UnityCommon;
using System;

/// <summary>
/// 2020-11-09 월 오후 3:39:48, 4.0.30319.42000, YONG-PC, Yong
/// </summary>
namespace UnityCommon
{
    public static class TypeCache<T>
    {
        public static readonly Type Type;

        static TypeCache()
        {
            Type = typeof(T);
        }
    }

    public static class TypeCache<TSrc, TDst>
    {
        public static readonly bool IsAssignableFrom;

        static TypeCache()
        {
            IsAssignableFrom = TypeCache<TSrc>.Type.IsAssignableFrom(TypeCache<TDst>.Type);
        }
    }
}