using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetRock : HitTargetBase
    {

        [SerializeField] private bool _CanOnlyDestroyByInteractType = false;
        [SerializeField] private float _Lifetime = 90f;
        [SerializeField] private float _DestroyDelayDuration = 3;
        [SerializeField] private string _HitEffectID = "";

        public UltEvents.UltEvent WhenCollideWithEnergyBall;

        public override void OnCollide(InteractType type, Collider selfCollider)
        {
            //if (!_CanOnlyDestroyByInteractType)
            //{
            //    ReceiveHit(selfCollider);
            //}
            //else
            //{
            //    if (type == interactType)
            //    {
            //        ReceiveHit(selfCollider);
            //    }
            //}
            base.OnCollide(type, selfCollider);
        }
        //private void ReceiveHit(Collider selfCollider)
        //{
        //    DestroyBullet(selfCollider);
        //    WhenCollide?.Invoke();
        //    WhenCollideUlt?.Invoke();
        //}
        private void OnEnable()
        {
            Destroy(gameObject, _Lifetime);

            OnEnergyBallInteract += BulletReact;
            OnShieldInteract += DestroyBullet;
            OnWhipInteract += DestroyBullet;
            OnBeamInteract += DestroyBullet;
        }
        private void OnDisable()
        {
            OnEnergyBallInteract -= BulletReact;
            OnShieldInteract -= DestroyBullet;
            OnWhipInteract -= DestroyBullet;
            OnBeamInteract -= DestroyBullet;
        }
        private void BulletReact(Collider selfCollider) {
            WhenCollideWithEnergyBall?.Invoke();

            Project.EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                _HitEffectID,
                false,
                _DestroyDelayDuration,
                transform.position));

            PlayRandomAudio();
        }
        private void DestroyBullet(Collider selfCollider)
        {
            WhenCollide?.Invoke();
            WhenCollideUlt?.Invoke();

            if (_UsephPushBackFeedback) PushBackFeedback(selfCollider);

            Project.EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                _HitEffectID,
                false,
                _DestroyDelayDuration,
                transform.position));

            PlayRandomAudio();

            if (TryGetComponent<MoveObject>(out var moveObj))
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
