using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace HelloPico2.LevelTool
{
    public class MoveOutLevelObject : MonoBehaviour
    {
        public Transform _Obj;
        public Animator _ObjAnimator;
        [Header("Thanks Player")]
        public float _Delay = 1f;
        public float _ThanksSpeed = 10f;
        public bool _UseLeftRightPlacer = true;
        public Vector3 _ThanksPlayerPosition = Vector3.zero;
        public float _FacingSpeed = 0.1f;
        public string _ThanksPlayerAnimationName = "isCheer";
        public float _ThanksDuration = 1f;
        public UltEvents.UltEvent _WhenThanksPlayer;

        [Header("Flyout")]
        public float _InitialDelay = 1f;
        public float _Speed = 100f;
        public float _FacingDuration = 1f;
        public AnimationCurve _XCurve;
        public AnimationCurve _YCurve;
        public AnimationCurve _ZCurve;
        public Vector3 _PositiveXTargetPos;
        public Vector3 _NegativeXTargetPos;
        public Vector3 _OffsetRange;

        [Header("Scale Settings")]
        [Range(0, 1)] public float _StartScalingTime;
        public AnimationCurve _ScalingEase;
        
        [Button]
        public void MoveOut(Transform obj) {
            _Obj = obj;
            MoveOut();
        }        
        public void MoveOut() {
            _Obj.transform.SetParent(_Obj.transform.root.parent);

            var thanksPlayerPos = _ThanksPlayerPosition;
            // pick left or right side of the player
            if (_UseLeftRightPlacer && _Obj.transform.position.x < 0) thanksPlayerPos.x *= -1;

            var targetPos = GetTargetWorldPosition();

            Sequence seq = DOTween.Sequence();

            #region Thanks Player
            seq.AppendInterval(_InitialDelay);

            var duration = Vector3.Distance(_Obj.transform.position, thanksPlayerPos) / _Speed;
            HelloPico2.LevelTool.FacingTarget facing = new FacingTarget(transform, 0.1f);

            duration = Vector3.Distance(_Obj.transform.position, thanksPlayerPos) / _ThanksSpeed;

            TweenCallback MoveToPlayerCallback = delegate {                
                facing = _Obj.gameObject.AddComponent<HelloPico2.LevelTool.FacingTarget>();
                facing._FacingThis = HelloPico2.Singleton.GameManagerHelloPico.Instance._Player.transform;
                facing._FacingSpeed = .3f;

                _Obj.DOMoveX(thanksPlayerPos.x, duration).SetEase(Ease.OutExpo);
                _Obj.DOMoveY(thanksPlayerPos.y, duration).SetEase(_YCurve);
                _Obj.DOMoveZ(thanksPlayerPos.z, duration).SetEase(_ZCurve);
            };
            seq.AppendCallback(MoveToPlayerCallback);
            seq.AppendInterval(duration); 
            
            TweenCallback ThanksPlayerCallback = delegate {
                _ObjAnimator.SetBool(_ThanksPlayerAnimationName, true);
                _WhenThanksPlayer?.Invoke();
            };
            seq.AppendCallback(ThanksPlayerCallback);
            seq.AppendInterval(_ThanksDuration);

            TweenCallback FinishedThanksPlayerCallback = delegate {
                _ObjAnimator.SetBool(_ThanksPlayerAnimationName, false);
            };
            seq.AppendCallback(FinishedThanksPlayerCallback);
            #endregion

            #region FlyOut
            seq.AppendInterval(_InitialDelay);            

            TweenCallback LeaveFacingCallback = delegate {
                if (facing != null) facing.UpdateToFacingPoint(targetPos);
            };
            seq.AppendCallback(LeaveFacingCallback);

            var durationMove = Vector3.Distance(thanksPlayerPos, targetPos) / _Speed;

            TweenCallback MoveCallback = delegate { 
                _Obj.DOMoveX(targetPos.x, durationMove).SetEase(_XCurve);
                _Obj.DOMoveY(targetPos.y, durationMove).SetEase(_YCurve);
                _Obj.DOMoveZ(targetPos.z, durationMove).SetEase(_ZCurve);
            };
            seq.AppendCallback(MoveCallback);
            seq.AppendInterval(_StartScalingTime * durationMove);
            seq.Append(_Obj.DOScale(0, (1 - _StartScalingTime) * durationMove));

            TweenCallback SeqCompleteCallback = delegate { Destroy(_Obj.gameObject); };
            seq.AppendCallback(SeqCompleteCallback);
            #endregion

            seq.Play();
        }
        private Vector3 GetTargetWorldPosition() {
            var offset = new Vector3( 
                Random.Range(-_OffsetRange.x, _OffsetRange.x),
                Random.Range(-_OffsetRange.y, _OffsetRange.y),
                Random.Range(-_OffsetRange.z, _OffsetRange.z)
                );

            if (_Obj.transform.position.x >= 0)
                return _PositiveXTargetPos + offset;
            else
                return _NegativeXTargetPos + offset;
        }
    }
}
