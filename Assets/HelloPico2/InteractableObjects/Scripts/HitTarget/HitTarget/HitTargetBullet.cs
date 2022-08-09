using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBullet : HitTargetBase
    {
        [SerializeField] private bool _CanOnlyDestroyByInteractType = false;
        [SerializeField] private float _Lifetime = 90f;        
        [SerializeField] private float _DestroyDelayDuration = 3;        
        [SerializeField] private string _HitEffectID = "";

        private void OnEnable()
        {
            Destroy(gameObject, _Lifetime);
        }
        public override void OnCollide(InteractType type, Collider selfCollider)
        {
            print("Collide " + type.ToString());

            if (!_CanOnlyDestroyByInteractType)
            {
                ReceiveHit(selfCollider);
            }
            else {
                if (type == interactType) {
                    ReceiveHit(selfCollider);
                }
            }
            base.OnCollide(type, selfCollider);
        }
        private void ReceiveHit(Collider selfCollider)
        {
            if (_UsephPushBackFeedback) PushBackFeedback(selfCollider);
            DestroyBullet(selfCollider);
            WhenCollide?.Invoke();
            WhenCollideUlt?.Invoke();
        }
        //private void OnEnable()
        //{
        //    OnEnergyBallInteract += DestroyBullet;
        //    OnShieldInteract += DestroyBullet;
        //    OnWhipInteract += DestroyBullet;
        //    OnBeamInteract += DestroyBullet;
        //}
        //private void OnDisable()
        //{
        //    OnEnergyBallInteract -= DestroyBullet;
        //    OnShieldInteract -= DestroyBullet;
        //    OnWhipInteract -= DestroyBullet;
        //    OnBeamInteract -= DestroyBullet;
        //}
        private void DestroyBullet(Collider selfCollider) {            
            Project.EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                _HitEffectID, 
                false, 
                _DestroyDelayDuration,
                transform.position));
            PlayRandomAudio();

            if(TryGetComponent<MoveObject>(out var moveObj))
                moveObj.speed = 0;

            Destroy(gameObject, _DestroyDelayDuration);
        }
        protected override void PushBackFeedback(Collider hitCol)
        {
            base.PushBackFeedback(hitCol);

            var targetPos = transform.position + hitCol.transform.forward * _PushBackDist;

            transform.DOMove(targetPos, _PushBackDuration).SetEase(_PushBackEasingCureve);
        }
    }
}
