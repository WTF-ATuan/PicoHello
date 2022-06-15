using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBullet : HitTargetBase
    {
        [SerializeField] private float _DestroyDelayDuration = 3;        
        [SerializeField] private string _HitEffectID = "";
        public override void OnCollide(InteractType type, Collider selfCollider)
        {
            print("Collide");
            WhenCollide?.Invoke();
            base.OnCollide(type, selfCollider);
        }
        private void OnEnable()
        {
            OnEnergyBallInteract += DestroyBullet;
            OnShieldInteract += DestroyBullet;
            OnWhipInteract += DestroyBullet;
            OnBeamInteract += DestroyBullet;
        }
        private void OnDisable()
        {
            OnEnergyBallInteract -= DestroyBullet;
            OnShieldInteract -= DestroyBullet;
            OnWhipInteract -= DestroyBullet;
            OnBeamInteract -= DestroyBullet;
        }
        private void DestroyBullet(Collider selfCollider) {            
            Project.EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                _HitEffectID, 
                false, 
                _DestroyDelayDuration,
                transform.position));
            PlayRandomAudio();

            Destroy(gameObject, _DestroyDelayDuration);             
        }
    }
}
