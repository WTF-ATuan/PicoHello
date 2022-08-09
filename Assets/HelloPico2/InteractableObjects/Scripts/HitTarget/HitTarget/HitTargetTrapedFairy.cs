using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetTrapedFairy : HitTargetBase
    {
        [SerializeField] private bool _CanOnlyDestroyByInteractType = false;
        [SerializeField] private float _Lifetime = 90f;
        [SerializeField] private float _DestroyDelayDuration = 3;
        [SerializeField] private string _HitEffectID = "";
        [SerializeField][ReadOnly] private int _DestroyCollideCount = 3;
        [SerializeField] private int _CollideCount = 0;
        [SerializeField] private GameObject _Fairy;
        public UltEvents.UltEvent WhenFirstCollide;
        public UltEvents.UltEvent WhenSecondCollide;
        public UltEvents.UltEvent WhenDestroy;

        public override void OnCollide(InteractType type, Collider selfCollider)
        {
            if (!_timer.CanInvoke()) return;

            base.OnCollide(type, selfCollider);

            if (!_CanOnlyDestroyByInteractType)
            {
                ReceiveHit(selfCollider);
            }
            else
            {
                if (type == interactType)
                {
                    ReceiveHit(selfCollider);
                }
            }

            _timer.Reset();
        }
        private void OnEnable()
        {
            Destroy(gameObject, _Lifetime);
        }
        private void ReceiveHit(Collider selfCollider)
        {
            _CollideCount++;

            if (_CollideCount < _DestroyCollideCount) {
                CageReact(selfCollider);
            }
            else
            {
                print("Destroy" + _CollideCount);
                DestroyCage(selfCollider);
            }
        }
        private void CageReact(Collider selfCollider)
        {
            if (_CollideCount == 1)
                WhenFirstCollide?.Invoke();
            if (_CollideCount == 2)
                WhenSecondCollide?.Invoke();

            Project.EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                _HitEffectID,
                false,
                _DestroyDelayDuration,
                transform.position));

            PlayRandomAudio();
        }
        private void DestroyCage(Collider selfCollider)
        {
            //if (_UsephPushBackFeedback) PushBackFeedback(selfCollider);

            WhenDestroy?.Invoke();

            Project.EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                _HitEffectID,
                false,
                _DestroyDelayDuration,
                transform.position));

            PlayRandomAudio();

            if (TryGetComponent<MoveObject>(out var moveObj))
                moveObj.speed = 0;

            Destroy(gameObject, _DestroyDelayDuration);

            var pos = _Fairy.transform.position;
            var rot = _Fairy.transform.rotation;
            var scale = _Fairy.transform.lossyScale;
            _Fairy.transform.SetParent(transform.parent, false);
            _Fairy.transform.position = pos;
            _Fairy.transform.rotation = rot;
            _Fairy.transform.localScale = scale;
        }
        protected override void PushBackFeedback(Collider hitCol)
        {
            base.PushBackFeedback(hitCol);

            var targetPos = transform.position + hitCol.transform.forward * _PushBackDist;

            transform.DOMove(targetPos, _PushBackDuration).SetEase(_PushBackEasingCureve);
        }
    }
}
