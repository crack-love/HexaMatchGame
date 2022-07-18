using System;
using System.Collections.Generic;

namespace UnityCommon
{
   static class ListEx
   {
      /// <summary>
      /// Set Count to size O(n)
      /// </summary>
      public static void SetSize<T>(this List<T> list, int size)
      {
         while (list.Count < list.Count) list.Add(default);
         while (list.Count > list.Count) list.RemoveAt(list.Count - 1);
         list.Capacity = size;
      }
   }
}
