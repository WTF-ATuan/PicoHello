using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PlayerController
{
    public class SpiritBehavior : MonoBehaviour
    {
        public Transform _GainArmorUpgradeRotationPivot;
        public float _OrbitDuration;
        private void Start()
        {
            HelloPico2.Singleton.GameManagerHelloPico.Instance._Spirit = this;
        }
        public void OnReceiveArmorUpgrade() { 
            
        }
    }
}
