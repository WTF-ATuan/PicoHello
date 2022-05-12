using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.InteractableObjects;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.PlayerController.Arm
{
    public class ArmData : MonoBehaviour
    {
        [SerializeField] private HandType _HandType;
        [SerializeField] private float _Energy;
        [SerializeField] private float _MaxEnergy;
        [SerializeField] private float _GrabEasingDuration;
        [SerializeField] private InteractableSettings _InteractableSettings;
        [SerializeField] private Transform _SummonPoint;
        public HandType HandType { get { return _HandType; } }
        public float Energy { get { return _Energy; } set { _Energy = value; } }
        public float MaxEnergy { get { return _MaxEnergy; } }
        public float GrabEasingDuration { get { return _GrabEasingDuration; } }
        public InteractableSettings InteractableSettings { get { return _InteractableSettings; } }
        public Transform SummonPoint { get { return _SummonPoint; } }

        public GameObject currentWeapon { get; set; }
    }
}
