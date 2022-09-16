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
        [SerializeField] private bool _UseVFX;
        [SerializeField] private ParticleSystem _VFX;

        [SerializeField] private HelloPico2.PlayerController.Arm.ArmorType _ArmorType;
        [SerializeField] private HelloPico2.PlayerController.Arm.ArmorPart _ArmorParts;

        [Header("Audio Settings")]
        [SerializeField] private string _GainClipName = "GainArmorUpgrade";
                
        public HelloPico2.PlayerController.Arm.ArmorType armorType { get { return _ArmorType; } }
        public HelloPico2.PlayerController.Arm.ArmorPart armorParts { get { return _ArmorParts; } }

        private Transform selectorTarget;

        bool used { get; set; }

        private void OnEnable()
        {
            if (transform.GetChild(1) == null) return;

            if (!_UseVFX) return;

            var clone = Instantiate(_VFX, transform.position, Quaternion.identity);
            clone.transform.SetParent(transform.GetChild(1));
        }
        GainEnergyEventData eventDate0;
        GainArmorUpgradeData eventDate;
        public override void OnSelect(DeviceInputDetected obj)
        {
            base.OnSelect(obj);
            
            OnAutoSelect();

            selectorTarget = obj.Selector.SelectorTransform;
        }
        public void OnAutoSelect() {
            if (used) return;

            StartArmorUpgradeSequence();
            
            // Hide Mesh
            foreach (var mesh in GetComponentsInChildren<Renderer>())
            {
                mesh.enabled = false;
            }
            
            // Disable collider
            GetComponent<Collider>().enabled = false;

            //ActivateArmor();

            AudioPlayerHelper.PlayAudio(_GainClipName, transform.position);

            used = true;
        }
        public void ActivateArmor() {
            // Add Armor
            GainArmorUpgradeData eventDate = new GainArmorUpgradeData();
            eventDate.armorType = _ArmorType;
            eventDate.armorPart = _ArmorParts;
            Project.EventBus.Post(eventDate);

            Destroy(gameObject);
        }
        private void Update()
        {
            if (selectorTarget == null) return;

            if (Vector3.Distance(transform.position, selectorTarget.position) < _StartScalingDist)
            {
                transform.DOScale(Vector3.zero, _ScalingDuration).SetEase(_EaseCurve);
            }
        }
        public void StartArmorUpgradeSequence()
        {
            var clone = Instantiate(transform.GetChild(1));
            clone.transform.position = transform.position;
            clone.SetParent(transform.root.parent);
            
            TweenCallback gainArmorCallback = () => {
                ActivateArmor();
            };
            HelloPico2.Singleton.ArmorUpgradeSequence.Instance.StartArmorUpgradeSequence(clone, gainArmorCallback);  
        }
        private void OnDisable()
        {

        }
    }
}
