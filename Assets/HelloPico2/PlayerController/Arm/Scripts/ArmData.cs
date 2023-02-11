using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Sirenix.OdinInspector;
using HelloPico2.InteractableObjects;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.PlayerController.Arm
{
    public class ArmData : MonoBehaviour
    {
        [SerializeField] private HandType _HandType;
        [SerializeField] private XRController _Controller;
        [SerializeField] private float _Energy;
        [SerializeField] private float _MaxEnergy;
        [SerializeField] private int _BombAmount;
        [SerializeField] private int _MaxBombAmount;
        [SerializeField] private float _GripFunctionEffectiveTime;
        [ReadOnly] public float currentGripFunctionTimer;// { get; set; }
        [SerializeField][Range(0f,30f)] private float _GrabDetectionRadiusMin = 0.1f;
        [SerializeField][Range(0.01f,30f)] private float _GrabDetectionRadius = 1;
        [HideInInspector] public float _GripDeadRange = 0.1f;
        public float originalGrabDetectionRadius { get; set; }
        [SerializeField][Min(0.01f)] private float _GrabDistance = 30;
        public float originalGrabDistance { get; set; }
        [SerializeField][Range(0.001f,1)] private float _GrabReleaseValue = 0.05f;
        [SerializeField] private float _GrabEasingDuration;
        [SerializeField] private AnimationCurve _GrabEasingCurve;
        [SerializeField] private InteractableSettings _InteractableSettings;
        [SerializeField] private Transform _SummonPoint;
        [SerializeField] private ArmorController _ArmorController;
        [SerializeField] private float _ShootEmptyEnergyCoolDownDuration = 2;
        [SerializeField] private float _ShootEmptyBombCoolDownDuration = 2;
        [SerializeField] private float _DisableInputCoolDownDuration = 1;
        [FoldoutGroup("Audio Settings")][SerializeField] private string[] _GainEnergyBallClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private float _RapidProjectileSoundInterval = 0.3f;
        [FoldoutGroup("Audio Settings")][SerializeField] private string[] _ShootEnergyBallClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string[] _ShootRapidEnergyBallClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _ToWhipClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _ToSwordClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _ToShieldClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _ShootWhenNoEnergyClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _ShootWhenNoBombClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _GainBombClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _ShootBombClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _EnergyballFullyCharged;

        [FoldoutGroup("Events Settings")] public UltEvents.UltEvent WhenGrip;
        [FoldoutGroup("Events Settings")] public UltEvents.UltEvent WhenNotGrip;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenGainEnergy;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenFullEnergy;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenGainBomb;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenTouchTriggerOrGrip;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenNotTouchTriggerAndGrip;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent<bool> WhenShootProjectile;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenShootChargedProjectile;
        [FoldoutGroup("Events Settings")] public UltEvents.UltEvent WhenNoEnergyShoot;
        [FoldoutGroup("Events Settings")] public UltEvents.UltEvent WhenNoBombShoot;
        public HandType HandType { get { return _HandType; } }
        public XRController Controller { get { return _Controller; } }
        public float Energy { get { return _Energy; } set { _Energy = Mathf.Clamp(value, 0, _MaxEnergy);} }
        public float MaxEnergy { get { return _MaxEnergy; } set { _MaxEnergy = value; } }
        public int bombAmount { get { return _BombAmount; } set { _BombAmount = Mathf.Clamp(value, 0, _MaxBombAmount); } }
        public int maxBombAmount{ get { return _MaxBombAmount; }set { _MaxBombAmount = value; } }
        public float gripFunctionEffectiveTime { get { return _GripFunctionEffectiveTime; } }
        public float GrabDetectionRadiusMin { get { return _GrabDetectionRadiusMin; } set { _GrabDetectionRadiusMin = value; } }
        public float GrabDetectionRadius { get { return _GrabDetectionRadius; } set { _GrabDetectionRadius = value; } }
        public float GrabDistance { get { return _GrabDistance; } set { _GrabDistance = value; } }
        public float GrabReleaseValue { get { return _GrabReleaseValue; } }
        public float GrabEasingDuration { get { return _GrabEasingDuration; } }
        public AnimationCurve GrabEasingCurve { get { return _GrabEasingCurve; } }
        public InteractableSettings InteractableSettings { get { return _InteractableSettings; } }
        public Transform SummonPoint { get { return _SummonPoint; } }
        public ArmorController ArmorController { get { return _ArmorController; } }
        public float DisableInputCoolDownDuration { get { return _DisableInputCoolDownDuration; } private set { _DisableInputCoolDownDuration = value; } }
        public string[] GainEnergyBallClipName { get { return _GainEnergyBallClipName; } }
        public float ShootEmptyEnergyCoolDownDuration { get { return _ShootEmptyEnergyCoolDownDuration; } }
        public float ShootEmptyBombCoolDownDuration { get { return _ShootEmptyBombCoolDownDuration; } }
        public float RapidProjectileSoundInterval { get { return _RapidProjectileSoundInterval; } }
        public string[] ShootEnergyBallClipName { get { return _ShootEnergyBallClipName; } }
        public string[] ShootRapidEnergyBallClipName { get { return _ShootRapidEnergyBallClipName; } }
        public string toWhipClipName { get{ return _ToWhipClipName; }}
        public string toSwordClipName { get{ return _ToSwordClipName; }}
        public string toShieldClipName { get{ return _ToShieldClipName; }}
        public string ShootWhenNoEnergyClipName { get{ return _ShootWhenNoEnergyClipName; }}
        public string ShootWhenNoBombClipName { get{ return _ShootWhenNoBombClipName; }}
        public string GainBombClipName { get{ return _GainBombClipName; }}
        public string ShootBombClipName{get{ return _ShootBombClipName; }}
        public string EnergyballFullyCharged { get{ return _EnergyballFullyCharged; }}
        public GameObject currentWeapon { get; set; }
        
    }
}
