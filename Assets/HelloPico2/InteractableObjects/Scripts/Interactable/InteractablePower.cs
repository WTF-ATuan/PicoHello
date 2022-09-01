using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InputDevice.Scripts;
using DG.Tweening;

namespace HelloPico2.InteractableObjects
{
    public class InteractablePower : InteractableBase
    {
        [SerializeField] private float _Energy = 20;
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

            // Charge Energy            
            TryGetComponent<IXRSelectInteractable>(out var Interactable);

            GainEnergyEventData eventDate = new GainEnergyEventData();
            eventDate.Energy = _Energy;
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
           
            if (_UseScaleControl)
            {
                if (Vector3.Distance(transform.position, selectorTarget.position) < _StartScalingDist)
                {
                    transform.DOScale(Vector3.zero, _ScalingDuration).SetEase(_EaseCurve);
                }
            }
        }
        private void OnDisable()
        {
            
        }
    }
}
