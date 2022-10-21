using Dreamteck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.Helper
{
    public class SortingHelper : MonoBehaviour
    {
        //public Transform TestOrigin;
        //public Transform[] TestTargets;
        //public Transform Result;
        //public void TestFindCloset() { 
        //    Result = FindClosest(TestTargets, TestOrigin);
        //}
        //public void TestFindAimingCloset()
        //{
        //    Result = FindAimingClosest(TestTargets, TestOrigin);
        //}
        public static Transform FindClosest(Transform[] sortTransform, Transform distCheckTransform) {
            if(sortTransform.Length == 0) return null;
            if(sortTransform.Length == 1) return sortTransform[0];

            float closetDist = float.MaxValue;
            float currentDist = 0;
            Transform result = sortTransform[0];

            for (int i = 0; i < sortTransform.Length; i++)
            {
                currentDist = Vector3.Distance(distCheckTransform.position, sortTransform[i].position);

                if (closetDist > currentDist) { 
                    closetDist = currentDist;
                    result = sortTransform[i];
                }
            }
            return result;
        }
        public static Transform FindAimingClosest(Transform[] sortTransform, Transform distCheckTransform)
        {
            if (sortTransform.Length == 0) return null;
            if (sortTransform.Length == 1) return sortTransform[0];

            float closetAngle = float.MaxValue;
            float currentAngle = 0;
            Transform result = sortTransform[0];
            Vector3 dir = Vector3.zero;

            for (int i = 0; i < sortTransform.Length; i++)
            {
                dir = (sortTransform[i].position - distCheckTransform.position).normalized;
                currentAngle = Vector3.Angle(dir, distCheckTransform.forward);
                print(sortTransform[i].name + " " + currentAngle);
                if (closetAngle > currentAngle)
                { 
                    closetAngle = currentAngle;
                    result = sortTransform[i];
                }
            }
            return result;
        }
    }
}
