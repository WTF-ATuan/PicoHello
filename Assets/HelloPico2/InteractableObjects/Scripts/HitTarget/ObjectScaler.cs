using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace HelloPico2.InteractableObjects
{
    public class ObjectScaler : MonoBehaviour
    {
        public enum ControlMode { BounceSequence, SingleBurst, BurstAndStay }
        [SerializeField] private ControlMode _ControlMode = ControlMode.BounceSequence;
        [SerializeField] private bool _RunOnEnable = false;
        [SerializeField] private GameObject _ScalingObject;
        [SerializeField] private Vector3 _From;
        [SerializeField] private Vector3 _To;
        [SerializeField] private float _Duration;
        [SerializeField] private int _Loop;
        [SerializeField] private AnimationCurve _EaseCurve;
        //[SerializeField] private Ease _EaseCurve;
        public ControlMode controlMode { get { return _ControlMode; } set { _ControlMode = value; } }
        public bool runOnEnable { get { return _RunOnEnable; } set { _RunOnEnable = value; } }
        public GameObject scalingObject { get { return _ScalingObject; } set { _ScalingObject = value; } }
        public Vector3 from { get { return _From; } set { _From = value; } }
        public Vector3 to { get { return _To; } set { _To = value; } }
        public float duration { get { return _Duration; } set { _Duration = value; } }
        public int loop { get { return _Loop; } set { _Loop = value; } }
        public AnimationCurve easeCurve { get { return _EaseCurve; } set { _EaseCurve = value; } }

        public UnityEvent WhenStart;
        public UnityEvent WhenFinished;

        public System.Action<GameObject> OnStarted;
        public System.Action<GameObject> OnFinished;

        private Vector3 defaultScale;
        
        public ObjectScaler (ControlMode controlMode, Vector3 from, Vector3 to, float duration, int loop, AnimationCurve easecurve) {
            _ControlMode = controlMode;
            _From = from;
            _To = to;
            _Duration = duration;
            _Loop = loop;
            _EaseCurve = easecurve;                
        }  

        private void Start()
        {
            defaultScale = _ScalingObject.transform.localScale;
        }
        private void OnEnable()
        {
            if(_RunOnEnable) StartScaling();
        }
        [Button]
        public void GetSelfAsScalingObj() {
            _ScalingObject = gameObject;
        }
        [Button]
        public void StartScaling(float rate)
        {
            var from = _ScalingObject.transform.localScale;
            var to = _To * rate;
            Scaling(from, to);
        }
        [Button]
        public void StartScaling()
        {
            Scaling(_From, _To);
        }
        [Button]
        public void SetFromScaleBasedOnCurrent(float mag) { 
            _From = _ScalingObject.transform.localScale * mag;
        }
        [Button]
        public void SetToScaleBasedOnCurrent(float mag)
        {
            _To = _ScalingObject.transform.localScale * mag;
        }
        private void Scaling(Vector3 from, Vector3 to) {
            Sequence seq = DOTween.Sequence();

            switch (_ControlMode)
            {
                case ControlMode.BounceSequence:
                    seq.Append(_ScalingObject.transform.DOScale(from, _Duration).SetEase(_EaseCurve));
                    seq.Append(_ScalingObject.transform.DOScale(to, _Duration).SetEase(_EaseCurve).SetLoops(_Loop * 2, LoopType.Yoyo));
                    seq.Append(_ScalingObject.transform.DOScale(defaultScale, _Duration).SetEase(_EaseCurve));
                    break;
                case ControlMode.SingleBurst:
                    seq.Append(_ScalingObject.transform.DOScale(to, _Duration).From(from).SetEase(_EaseCurve).SetLoops(_Loop, LoopType.Yoyo));
                    seq.Append(_ScalingObject.transform.DOScale(defaultScale, _Duration).SetEase(_EaseCurve).SetLoops(_Loop, LoopType.Yoyo));
                    break;
                case ControlMode.BurstAndStay:
                    seq.Append(_ScalingObject.transform.DOScale(to, _Duration).From(from).SetEase(_EaseCurve).SetLoops(_Loop, LoopType.Yoyo));
                    break;
                default:
                    break;
            }
            WhenStart?.Invoke(); OnStarted?.Invoke(gameObject);
            seq.Play().OnComplete(() => { WhenFinished?.Invoke(); OnFinished?.Invoke(gameObject); });
        }
    }
}