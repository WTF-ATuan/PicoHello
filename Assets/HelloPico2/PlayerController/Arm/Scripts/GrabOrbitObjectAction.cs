using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace HelloPico2.PlayerController
{
    public class GrabOrbitObjectAction : MonoBehaviour, IOribitMovement
    {
        public bool _UseTween = false;
        [ShowIf("_UseTween")] public Transform _Pivot;
        [ShowIf("_UseTween")] public float _GrabbingDuration = 1;
        [ShowIf("_UseTween")] public AnimationCurve _GrabbingEaseX;
        [ShowIf("_UseTween")] public AnimationCurve _GrabbingEaseY;
        [ShowIf("_UseTween")] public AnimationCurve _GrabbingEaseZ;
        [ShowIf("_UseTween")] public Vector3 _StartPosition = new Vector3(0,0,10);

        [HideIf("_UseTween")] public Animator _Animator;
        [HideIf("_UseTween")] public string _GrabObjectName;
        [HideIf("_UseTween")] public Transform _AnimationPivot;
        public Vector3 _Rotation;
        public float _Duration;

        Vector3 originalScale;
        
        public void Move(GameObject obj, Action callback)
        {
            if (!_UseTween)
                AnimationMove(obj, callback);
            else
                TweenMove(obj, callback);
        }
        public void AnimationMove(GameObject obj, Action callback)
        {
            obj.transform.SetParent(_AnimationPivot, false);
            originalScale = obj.transform.localScale;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = _Rotation;
            //obj.transform.localScale = Vector3.one;

            _Animator.SetTrigger(_GrabObjectName);

            StartCoroutine(Delayer(obj, callback));
        }
        public void TweenMove(GameObject obj, Action callback)
        {
            obj.transform.SetParent(_Pivot, false);
            originalScale = obj.transform.localScale;
            obj.transform.localEulerAngles = _Rotation;

            Sequence seq = DOTween.Sequence();

            TweenCallback Grabbing = () =>
            {
                obj.transform.DOLocalMoveX(0, _GrabbingDuration).From(_StartPosition.x).SetEase(_GrabbingEaseX);
                obj.transform.DOLocalMoveY(0, _GrabbingDuration).From(_StartPosition.y).SetEase(_GrabbingEaseY);
                obj.transform.DOLocalMoveZ(0, _GrabbingDuration).From(_StartPosition.z).SetEase(_GrabbingEaseZ);
            };

            seq.AppendCallback(Grabbing);
            seq.AppendInterval(_Duration);

            TweenCallback Finished = () =>
            {
                callback?.Invoke();
            };

            seq.AppendCallback(Finished);
        }
        private IEnumerator Delayer(GameObject obj, Action callback) {
            yield return new WaitForSeconds (_Duration);
            callback?.Invoke();
        }
    }
    public interface IOribitMovement { 
        public void Move(GameObject obj, System.Action callback);
    }
}
