using System.Collections;
using System.Collections.Generic;
using Project;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBell : HitTargetBase
    {
        [SerializeField] private float _Force;
        [SerializeField] private Transform _ForceOrigin;
        [SerializeField] private Rigidbody _Rigidbody;
        
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

            PlayAudio();

            var dir = (_ForceOrigin.position - _Rigidbody.transform.position).normalized;
            _Rigidbody.AddForce(_Force * dir, ForceMode.VelocityChange);
        }
        private void PlayAudio()
        {
            //var value = Random.Range(0, 2);
            //string result = "";
            //if (value == 0)
            //    result = _ChargeBloomClipName0;
            //else
            //    result = _ChargeBloomClipName1;

            //EventBus.Post(new AudioEventRequested(result, transform.position));
        }
    }
}