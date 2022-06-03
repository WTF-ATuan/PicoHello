using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class ObjectMover : MonoBehaviour
    {
        [SerializeField] private GameObject _MovingObject;
        [SerializeField] private Vector3 _Offset;
        [SerializeField] private float _Duration;
        [SerializeField] private AnimationCurve _EasingCurve;
        [SerializeField] private bool _TriggerWhenEnable = true;
        private TweenerCore<Vector3, Vector3, VectorOptions> tween;

        private void OnEnable()
        {
            StartMoving();
        }
        private void OnDisable()
        {
            tween.Kill();
        }
        [Button]
        public void UpdateSettings()
        {
            OnDisable();
            OnEnable();
        }
        private void StartMoving() {
            tween = _MovingObject.transform.DOLocalMoveY(_Offset.y, _Duration).SetEase(_EasingCurve).SetLoops(int.MaxValue);            
        }
    }
}
