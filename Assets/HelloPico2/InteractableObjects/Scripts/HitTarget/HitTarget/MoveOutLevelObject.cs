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
        public float _InitialDelay = 1f;
        public float _Speed = 10f;
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
            var targetPos = GetTargetWorldPosition();
            var duration = Vector3.Distance(_Obj.transform.position, targetPos) / _Speed;

            Sequence seq = DOTween.Sequence();

            seq.AppendInterval(_InitialDelay);

            TweenCallback MoveCallback = delegate { 
                _Obj.DOMoveX(targetPos.x, duration).SetEase(_XCurve);
                _Obj.DOMoveY(targetPos.y, duration).SetEase(_YCurve);
                _Obj.DOMoveZ(targetPos.z, duration).SetEase(_ZCurve);
            };
            seq.AppendCallback(MoveCallback);

            seq.AppendInterval(_StartScalingTime * duration);
            seq.Append(_Obj.DOScale(0, (1 - _StartScalingTime) * duration));
            TweenCallback SeqCompleteCallback = delegate { Destroy(_Obj.gameObject); };
            seq.AppendCallback(SeqCompleteCallback);
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
