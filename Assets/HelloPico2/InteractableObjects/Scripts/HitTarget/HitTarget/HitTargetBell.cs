using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBell : HitTargetBase
    {
        public override void OnCollide(InteractType type)
        {
            base.OnCollide(type);
        }
        private void OnEnable()
        {
            OnEnergyBallInteract += BellActivate;
            OnBeamInteract += BellActivate;            
        }
        private void OnDisable()
        {
            OnEnergyBallInteract -= BellActivate;
            OnBeamInteract -= BellActivate;            
        }
        private void BellActivate()
        {
            WhenCollide?.Invoke();

            //PlayAudio();
        }
    }
}