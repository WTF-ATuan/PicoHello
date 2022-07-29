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
        [SerializeField][Range(0.01f,30f)] private float _GrabDetectionRadius = 1;
        [SerializeField][Min(0.01f)] private float _GrabDistance = 30;
        [SerializeField] private float _GrabEasingDuration;
        [SerializeField] private InteractableSettings _InteractableSettings;
        [SerializeField] private Transform _SummonPoint;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _GainEnergyBallClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _ShootEnergyBallClipName;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _ShootChargedEnergyBallClipName;

        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenGainEnergy;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenTouchTriggerOrGrip;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenNotTouchTriggerAndGrip;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenShootProjectile;
        [FoldoutGroup("Events Settings")] public UnityEngine.Events.UnityEvent WhenShootChargedProjectile;
        public HandType HandType { get { return _HandType; } }
        public XRController Controller { get { return _Controller; } }
        public float Energy { get { return _Energy; } set { _Energy = Mathf.Clamp(value, 0, _MaxEnergy);} }
        public float MaxEnergy { get { return _MaxEnergy; } }
        public float GrabDetectionRadius { get { return _GrabDetectionRadius; } }
        public float GrabDistance { get { return _GrabDistance; } }
        public float GrabEasingDuration { get { return _GrabEasingDuration; } }
        public InteractableSettings InteractableSettings { get { return _InteractableSettings; } }
        public Transform SummonPoint { get { return _SummonPoint; } }
        public string GainEnergyBallClipName { get { return _GainEnergyBallClipName; } }
        public string ShootEnergyBallClipName { get { return _ShootEnergyBallClipName; } }
        public string ShootChargedEnergyBallClipName{get{ return _ShootChargedEnergyBallClipName; }}
        public GameObject currentWeapon { get; set; }
    }
}
