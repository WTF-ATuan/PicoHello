using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;


namespace HelloPico2.InteractableObjects
{
    public class ObjectScaler : MonoBehaviour
    {
        public enum ControlMode { BounceSequence, SingleBurst, BurstAndStay }
        [SerializeField] private ControlMode _ControlMode = ControlMode.BounceSequence;
        [SerializeField] private GameObject _ScalingObject;
        [SerializeField] private Vector3 _From;
        [SerializeField] private Vector3 _To;
        [SerializeField] private float _Duration;
        [SerializeField] private int _Loop;
        [SerializeField] private AnimationCurve _EaseCurve;
        //[SerializeField] private Ease _EaseCurve;

        private Vector3 defaultScale;

        private void Start()
        {
            defaultScale = _ScalingObject.transform.localScale;
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

            seq.Play();
        }
    }
}