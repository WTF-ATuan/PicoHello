using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PlayerController
{
    public class GrabOrbitObjectAction : MonoBehaviour, IOribitMovement
    {
        public Animator _Animator;
        public string _GrabObjectName;
        public Transform _AnimationPivot;
        public Vector3 _Rotation;
        public float _Duration;

        Vector3 originalScale;
        
        public void Move(GameObject obj, Action callback)
        {
            obj.transform.SetParent(_AnimationPivot, false);
            originalScale = obj.transform.localScale;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = _Rotation;
            //obj.transform.localScale = Vector3.one;

            _Animator.SetTrigger(_GrabObjectName);

            StartCoroutine(Delayer(obj, callback));
        }
        private IEnumerator Delayer(GameObject obj, Action callback) {
            yield return new WaitForSeconds (_Duration);
            callback?.Invoke();
            //obj.transform.localScale = originalScale;
        }
    }
    public interface IOribitMovement { 
        public void Move(GameObject obj, System.Action callback);
    }
}
