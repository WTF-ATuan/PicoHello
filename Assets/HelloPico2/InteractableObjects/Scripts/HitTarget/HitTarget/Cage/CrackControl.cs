using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class CrackControl : MonoBehaviour
    {
        [SerializeField] private int _HitCountEachStep = 3;
        [SerializeField] private int _TotalSteps = 10;
        [Header("Emission Settings")]
        [SerializeField] private EmissionRaiseSteps _EmissionControl; 
        [SerializeField] private EmissionRaiseSteps _EmissionColorControl; 
        [SerializeField] private float _Duration;         
        [SerializeField] private int _MatID;         

        private int CurrentHitCount;
        private int CurrentStep;

        [Button("ReceiveHit")]
        public void ReceiveHit() {
            if (CurrentHitCount < _HitCountEachStep)
                CurrentHitCount++;
            else
            {
                CurrentHitCount = 0;

                if (CurrentStep < _TotalSteps)
                {
                    CurrentStep++;
                    NotifyCrackControl();
                }
            }
        }
        private void NotifyCrackControl() {
            //_EmissionColorControl.RaiseToColor(0, false, false, 0.1f, _MatID);
            _EmissionControl.RaiseToValue(CurrentStep, false, false, _Duration, _MatID);
        }
    }
}
