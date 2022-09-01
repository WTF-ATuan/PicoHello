using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InputDevice.Scripts;
using DG.Tweening;

namespace HelloPico2.InteractableObjects
{
    public class InteractableBomb : InteractableBase
    {
        [SerializeField] private int _BombAmount = 1;
        [Header("Merge into Charging ball Settings")]
        [SerializeField] private bool _UseScaleControl = true;
        [SerializeField] private float _StartScalingDist = 10;
        [SerializeField] private float _ScalingDuration = .5f;
        [SerializeField] private AnimationCurve _EaseCurve;

        private Transform selectorTarget;

        bool used { get; set; }
        public override void OnSelect(DeviceInputDetected obj)
        {
            base.OnSelect(obj);
            if (used) return;

            TryGetComponent<IXRSelectInteractable>(out var Interactable);

            GainBombEventData eventDate = new GainBombEventData();
            eventDate.Amount = _BombAmount;
            eventDate.Interactable = Interactable;
            eventDate.InputReceiver = obj;
            Project.EventBus.Post(eventDate);

            GetComponent<Collider>().enabled = false;
            used = true;

            selectorTarget = obj.Selector.SelectorTransform;
        }
        private void Update()
        {
            if (selectorTarget == null) return;
            
            // Scaling down when approaching to player
            if (_UseScaleControl)
            {
                if (Vector3.Distance(transform.position, selectorTarget.position) < _StartScalingDist)
                {
                    transform.DOScale(Vector3.zero, _ScalingDuration).SetEase(_EaseCurve);
                }
            }
        }
    }
}
