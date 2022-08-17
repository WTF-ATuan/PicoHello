using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace HelloPico2.LevelTool
{
    public class FloatingObjectMover : MonoBehaviour
    {
        public Vector2 _DepthRange = new Vector2(10,15);
        public Vector2 _TiltRotationRange = new Vector2(-30,30);
        public Vector2 _VerticalRotationRange = new Vector2(-60,60);
        public float _EndDepthValue = 5;
        public float _DepthDuration = 100;
        public float _TiltDuration = 30;
        public float _HorizontalSpeed = 5;
        public float _VerticalDuration = 20;
        public Ease _TiltEase = Ease.InOutCubic;
        public Ease _VerticalEase = Ease.InOutCubic;
        public Transform _Container;
        public List<GameObject> _FloatingObjectList = new List<GameObject>();

        private void SetUpFloatingObjects() {
            for (int i = 0; i < _FloatingObjectList.Count; i++)
            {
                SetUpFloating(_FloatingObjectList[i].transform);
            }
        }

        [Button]
        private void SetUpFloating(Transform obj) {
            var tiltPivot =  Instantiate(new GameObject(), _Container);
            var horizontalPivot =  Instantiate(new GameObject(), _Container);
            var verticalPivot =  Instantiate(new GameObject(), _Container);
            var depth = Random.Range(_DepthRange.x, _DepthRange.y);
            var hAngle = Random.Range(0, 360);
            var vAngle = Random.Range(_VerticalRotationRange.x, _VerticalRotationRange.y);

            obj.SetParent(verticalPivot.transform);
            verticalPivot.transform.SetParent(horizontalPivot.transform);
            horizontalPivot.transform.SetParent(tiltPivot.transform);

            obj.transform.localPosition = new Vector3(0,0,depth);
            tiltPivot.transform.localEulerAngles = new Vector3(0,hAngle,0);
            horizontalPivot.transform.localEulerAngles = new Vector3(0,hAngle,0);
            verticalPivot.transform.localEulerAngles = new Vector3(vAngle,0,0);

            // horizontal
            var rotControl = horizontalPivot.AddComponent<RotateObject>();
            rotControl.rotateY = _HorizontalSpeed;
            // tilt
            Sequence tiltSeq = DOTween.Sequence();
            var tiltDuration = Mathf.Abs(tiltPivot.transform.localEulerAngles.x - _TiltRotationRange.y) * _TiltDuration / Mathf.Abs(_TiltRotationRange.y - _TiltRotationRange.x);
            tiltSeq.Append(tiltPivot.transform.DOLocalRotate(new Vector3(_TiltRotationRange.y, 0, 0), tiltDuration).From(tiltPivot.transform.localEulerAngles).OnComplete(() => {
                tiltPivot.transform.DOLocalRotate(new Vector3(_TiltRotationRange.x, 0, 0), _VerticalDuration)
                .From(new Vector3(_TiltRotationRange.y, 0, 0))
                .SetLoops(int.MaxValue, LoopType.Yoyo)
                .SetEase(_TiltEase);
            }));
            tiltSeq.Play();
            // vertical
            Sequence verticalSeq = DOTween.Sequence();
            var verticalDuration = Mathf.Abs(verticalPivot.transform.localEulerAngles.x - _VerticalRotationRange.y) * _VerticalDuration / Mathf.Abs(_VerticalRotationRange.y - _VerticalRotationRange.x);
            verticalSeq.Append(verticalPivot.transform.DOLocalRotate(new Vector3(_VerticalRotationRange.y, 0, 0), verticalDuration).From(verticalPivot.transform.localEulerAngles).OnComplete(() => {
                verticalPivot.transform.DOLocalRotate(new Vector3(_VerticalRotationRange.x, 0, 0), _VerticalDuration)
                .From(new Vector3(_VerticalRotationRange.y, 0, 0))
                .SetLoops(int.MaxValue, LoopType.Yoyo)
                .SetEase(_VerticalEase);
            }));
            verticalSeq.Play();

            Sequence endSeq = DOTween.Sequence();
            endSeq.Append(obj.transform.DOLocalMoveZ(_EndDepthValue, _DepthDuration).SetEase(Ease.InOutCubic));
            TweenCallback StopHorizontalCallback = () => {
                tiltSeq.Kill();
                verticalSeq.Kill();

                float yValue = rotControl.rotateY;                
                DOTween.To(() => yValue, x => yValue = x, 0, .5f).OnUpdate(() => rotControl.rotateY = yValue);

                var horizontalDuration1 = horizontalPivot.transform.localEulerAngles.y * _VerticalDuration / 360;
                horizontalPivot.transform.DOLocalRotate(Vector3.zero, horizontalDuration1, RotateMode.Fast).SetEase(Ease.Linear).OnComplete(() => { 
                    obj.SetParent(_Container);
                    Destroy(tiltPivot);

                    var target = transform.position + transform.forward * _EndDepthValue;
                    var endDuration = Vector3.Distance(obj.transform.position, target);
                    obj.transform.DOMove(target, endDuration);                
                });

            };
            endSeq.AppendCallback(StopHorizontalCallback);            
            endSeq.Play();
        }
    }
}
