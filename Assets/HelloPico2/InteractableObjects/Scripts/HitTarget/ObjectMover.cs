using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace HelloPico2.InteractableObjects
{
    public class ObjectMover : MonoBehaviour
    {
        [SerializeField] private bool _RunOnEnable = false;
        [SerializeField] private Transform _MoveObject;
        [SerializeField] private Vector3 _From;
        [SerializeField] private Vector3 _To;
        [SerializeField] private float _Duration;
        [SerializeField] private AnimationCurve _EaseCurve;
        [SerializeField] private bool _UseLoop = false;
        [ShowIf("_UseLoop")][Range(2,999)][Tooltip("999 equals infinity")][SerializeField] private int _LoopAmount = 2;
        [ShowIf("_UseLoop")][SerializeField] private LoopType _LoopType = LoopType.Yoyo;

        public Transform moveObject { get { return _MoveObject; } set { _MoveObject = value; } }
        public Vector3 from { get { return _From; } set { _From = value; } }
        public Vector3 to { get { return _To; } set { _To = value; } }
        public float duration { get { return _Duration; } set { _Duration = value; } }
        public AnimationCurve easeCurve { get { return _EaseCurve; } set { _EaseCurve = value; } }
        
        public UnityEvent WhenStart;
        public UnityEvent WhenFinished;

        private Vector3 defaultPos;

        private void Start()
        {
            defaultPos = _MoveObject.localPosition;
        }
        private void OnEnable()
        {
            if(_RunOnEnable) StartMoving();
        }
        [Button]
        public void GetSelfAsMovingObj()
        {
            _MoveObject = transform;
        }
        [Button]
        public void StartMoving(float rate)
        {
            var from = _MoveObject.transform.localPosition;
            var to = _To * rate;
            Moving(from, to);
        }
        [Button]
        public void StartMoving()
        {
            Moving(_From, _To);
        }
        [Button]
        public void SetFromPosBasedOnCurrent(Vector3 offset)
        {
            _From = _MoveObject.transform.localPosition + offset;
        }
        [Button]
        public void SetToPosBasedOnCurrent(Vector3 offset)
        {
            _To = _MoveObject.transform.localPosition + offset;
        }
        private void Moving(Vector3 from, Vector3 to)
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(_MoveObject.DOLocalMove(to, _Duration).From(from).SetEase(_EaseCurve));

            if (_UseLoop)
            {
                var amount = (_LoopAmount == 999)? int.MaxValue : _LoopAmount;
                seq.SetLoops(amount, _LoopType);
            }

            WhenStart?.Invoke();
            seq.Play().OnComplete(() => { WhenFinished?.Invoke(); });            
        }        
    }
}
