using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InputDevice.Scripts;
using DG.Tweening;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class InteractableArmorUpgrade : InteractableBase
    {
        [Header("Merge into Charging ball Settings")]
        private float _Energy = 0;
        [SerializeField] private float _StartScalingDist = 10;
        [SerializeField] private float _ScalingDuration = .5f;
        [SerializeField] private AnimationCurve _EaseCurve;
        [SerializeField] private ParticleSystem _VFX;

        [SerializeField] private HelloPico2.PlayerController.Arm.ArmorType _ArmorType;
        [SerializeField] private HelloPico2.PlayerController.Arm.ArmorPart _ArmorParts;

        private Transform selectorTarget;

        bool used { get; set; }

        private void OnEnable()
        {
            if (transform.GetChild(1) == null) return;
            var clone = Instantiate(_VFX, transform.position, Quaternion.identity);
            clone.transform.SetParent(transform.GetChild(1));
        }

        public override void OnSelect(DeviceInputDetected obj)
        {
            base.OnSelect(obj);
            if (used) return;

            // Charge Energy            
            TryGetComponent<IXRSelectInteractable>(out var Interactable);

            GainEnergyEventData eventDate0 = new GainEnergyEventData();
            eventDate0.Energy = _Energy;
            eventDate0.Interactable = Interactable;
            eventDate0.InputReceiver = obj;
            Project.EventBus.Post(eventDate0);

            // Add Armor
            GainArmorUpgradeData eventDate = new GainArmorUpgradeData();
            eventDate.armorType = _ArmorType;  
            eventDate.armorPart = _ArmorParts;
            Project.EventBus.Post(eventDate);

            GetComponent<Collider>().enabled = false;
            used = true;

            selectorTarget = obj.Selector.SelectorTransform;
        }
        private void Update()
        {
            if (selectorTarget == null) return;

            if (Vector3.Distance(transform.position, selectorTarget.position) < _StartScalingDist)
            {
                transform.DOScale(Vector3.zero, _ScalingDuration).SetEase(_EaseCurve);
            }
        }
        private void OnDisable()
        {

        }
    }
}
