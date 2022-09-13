using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PlayerController
{
    public class GainArmorEventListener : MonoBehaviour
    {
        public UltEvents.UltEvent WhenGainArmor;
        private void OnEnable()
        {
            Project.EventBus.Subscribe<GainArmorEvent>(OnNotify);
        }
        private void OnNotify(GainArmorEvent gainarmorEvent) {
            WhenGainArmor?.Invoke();
        }
    }
}
