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
        [SerializeField][Range(0.01f,30f)] private float _GrabDetectionRadiusMin = 0.1f;
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
        [FoldoutGroup("Audio Settings")][SerializeField] private string[] _GainEnergyBallClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string[] _ShootEnergyBallClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _ShootChargedEnergyBallClipName;

        [FoldoutGroup("Events Settings")] public UltEvents.UltEvent WhenGrip;
        [FoldoutGroup("Events Settings")] public UltEvents.UltEvent WhenNotGrip;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenGainEnergy;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenGainBomb;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenTouchTriggerOrGrip;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenNotTouchTriggerAndGrip;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenShootProjectile;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenShootChargedProjectile;
        public HandType HandType { get { return _HandType; } }
        public XRController Controller { get { return _Controller; } }
        public float Energy { get { return _Energy; } set { _Energy = Mathf.Clamp(value, 0, _MaxEnergy);} }
        public float MaxEnergy { get { return _MaxEnergy; } }
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
        public string[] GainEnergyBallClipName { get { return _GainEnergyBallClipName; } }
        public string[] ShootEnergyBallClipName { get { return _ShootEnergyBallClipName; } }
        public string ShootChargedEnergyBallClipName{get{ return _ShootChargedEnergyBallClipName; }}
        public GameObject currentWeapon { get; set; }
        
    }
}
