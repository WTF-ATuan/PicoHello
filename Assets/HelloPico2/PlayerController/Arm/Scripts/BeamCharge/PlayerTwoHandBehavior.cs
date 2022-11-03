using HelloPico2.PlayerController.Arm;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PlayerController.BeamCharge
{
    public class PlayerTwoHandBehavior : MonoBehaviour
    {
        public string _PlayersHandTag = "Interactable";
        [ReadOnly] public GameObject[] _PlayersHands;
        public PickableEnergy[] _PickableEnergys;

        private void Awake()
        {
            _PlayersHands = GameObject.FindGameObjectsWithTag(_PlayersHandTag);
        }

        private void OnEnable()
        {
            foreach (var energy in _PickableEnergys)
            {
                energy.OnPlayerGetEnergy += StoreEnergyOnHand;
            }
        }
        private void OnDisable()
        {
            foreach (var energy in _PickableEnergys)
            {
                energy.OnPlayerGetEnergy -= StoreEnergyOnHand;
            }
        }

        private void StoreEnergyOnHand(PickableEnergy energy, InteractCollider handCol)
        {
            
        }
    }
}
