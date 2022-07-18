using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   [ExecuteAlways]
   class TaskWatcher : UnityCommon.MonoBehaviourSingletone<TaskWatcher>
   {
      LinkedList<Task> m_list = new LinkedList<Task>();

      private void Update()
      {
         if (m_list.Count < 0) return;
         var node = m_list.First;
         
         while (node != null)
         {
            var task = node.Value;

            // Log Exception 
            if (task.IsFaulted)
            {
               Debug.LogException(task.Exception);
            }

            var prev = node;
            node = node.Next;

            if (task.IsCompleted)
            {
               m_list.Remove(prev);
            }
         }
      }

      public void WatchTask(Task task)
      {
         m_list.AddLast(task);
      }
   }

   static class TaskEx
   {
      /// <summary>
      /// Dismiss Task and watch fault
      /// </summary>
      public static void Watch(this Task src)
      {
         TaskWatcher.Instance.WatchTask(src);
      }
   }
}