using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace YGK
{
   /// <summary>
   /// 심플 애니메이션의 줄임으로 간단한 이동 애니메이션을 추상화해서 사용할 수 있도록 지원한다. 
   /// </summary>
   class Simation
   {
      public static AnimationCurve LinearCurve = AnimationCurve.Linear(0, 0, 1, 1);
      public static AnimationCurve EaseInOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
      public static AnimationCurve DefaultCurve = LinearCurve;

      public Vector3 Begin;
      public Vector3 End;
      public Transform Target;
      public AnimationCurve Curve;
      public float DurationSec;
      public float PastTimeSec;
      public Task AsyncTask;
      public bool IsCanceled;

      public Simation(Vector3 end, Transform target, float durationSec) :
         this(target.transform.position, end, target, DefaultCurve, durationSec)
      { }

      public Simation(Vector3 beg, Vector3 end, Transform target, float durationSec) :
         this(beg, end, target, DefaultCurve, durationSec)
      { }

      public Simation(Vector3 beg, Vector3 end, Transform target, AnimationCurve curve, float durationSec)
      {
         Begin = beg;
         End = end;
         Target = target;
         Curve = curve;
         DurationSec = durationSec;
         PastTimeSec = 0;
         AsyncTask = null;
      }

      public void Start()
      {
         AsyncTask = StartAsync();
      }

      public void Cancel()
      {
         IsCanceled = true;
      }

      public async Task StartAsync()
      {
         PastTimeSec = 0;

         if (IsCanceled)
         {
            return;
         }

         while (PastTimeSec < DurationSec)
         {
            float p = PastTimeSec / DurationSec;
            p = Curve.Evaluate(p);

            Vector3 pos = Vector3.LerpUnclamped(Begin, End, p);
            if (!Target) return;
            Target.position = pos;

            await Task.Yield();
            PastTimeSec += Time.deltaTime;

            if (IsCanceled)
            {
               return;
            }
         }

         Target.position = End;
      }
   }
}