using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;


namespace HelloPico2.InteractableObjects
{
    public class ObjectScaler : MonoBehaviour
    {
        [SerializeField] private GameObject _ScalingObject;
        [SerializeField] private Vector3 _From;
        [SerializeField] private Vector3 _To;
        [SerializeField] private float _Duration;
        [SerializeField] private int _Loop;
        [SerializeField] private AnimationCurve _EaseCurve;

        private Vector3 defaultScale;

        private void Start()
        {
            defaultScale = _ScalingObject.transform.localScale;
        }
        [Button]
        public void StartScaling()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(_ScalingObject.transform.DOScale(_From, _Duration).SetEase(_EaseCurve));
            seq.Append(_ScalingObject.transform.DOScale(_To, _Duration).SetEase(_EaseCurve).SetLoops(_Loop * 2, LoopType.Yoyo));
            seq.Append(_ScalingObject.transform.DOScale(defaultScale, _Duration).SetEase(_EaseCurve));

            seq.Play();
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
    }
}