using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.InteractableObjects;

namespace HelloPico2.PlayerController.Arm
{
    public class ArmData : MonoBehaviour
    {
        [SerializeField] private float _Energy;
        [SerializeField] private float _MaxEnergy;
        [SerializeField] private InteractableSettings _InteractableSettings;
        [SerializeField] private Transform _SummonPoint;
        public float Energy { get { return _Energy; } set { _Energy = value; } }
        public float MaxEnergy { get { return _MaxEnergy; } }
        public InteractableSettings InteractableSettings { get { return _InteractableSettings; } }
        public Transform SummonPoint { get { return _SummonPoint; } }

        public GameObject currentWeapon { get; set; }
    }
}
