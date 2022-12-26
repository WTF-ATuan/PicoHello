using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UltEvents;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetChain : HitTargetBase
    {
        [System.Serializable] 
        public class HitCountSettings {
            public InteractType InteractType;
            public int HitCount;
        };
        [SerializeField] private List<HitCountSettings> _HitCountSettings = new List<HitCountSettings>();
        [SerializeField] private bool _GenerateHitVFX = true;
        [SerializeField] private string _HitEffectID = "";
        [SerializeField] private bool _UseDestroySFX = false;
        [ShowIf("_UseDestroySFX")][SerializeField] private int[] _NormalAudioIndex;
        [ShowIf("_UseDestroySFX")][SerializeField] private int[] _DestroyAudioIndex;
        [SerializeField] private bool _DestroyAfterHit = true;
        [ShowIf("_DestroyAfterHit")][SerializeField] private float _DestroyDelayDuration = 3;
        public UltEvent WhenDestroy;
        public List<HitCountSettings> originalHitCountSettings = new List<HitCountSettings>();
        private void Awake()
        {
            AssignHitCountSettings(_HitCountSettings, originalHitCountSettings);
        }
        public override void OnCollide(InteractType type, Collider selfCollider)
        {
            _timer ??= new Game.Project.ColdDownTimer(_HitCDDuration);
            if (!_timer.CanInvoke()) return;
            _timer.Reset();
            OnInteract(type, selfCollider); 
        }
        private void OnInteract(InteractType type, Collider selfCollider)
        {
            var allowHit = CheckAllowHit(type);
            print("allowHit"+allowHit);
            if (!allowHit.allowHit) return;

            var currentInteractTypeHitCount = UpdateHitCount(allowHit.hitCountSettings, 1);

            if (currentInteractTypeHitCount > 0)            
                HitReaction(selfCollider);            
            else
                DestroyReaction(selfCollider);
        }
        private (bool allowHit, HitCountSettings hitCountSettings) CheckAllowHit(InteractType type) {
            for (int i = 0; i < _HitCountSettings.Count; i++)
            {
                if (_HitCountSettings[i].InteractType == type)                
                    return (true, _HitCountSettings[i]);                 
            }
            return (false, _HitCountSettings[0]);
        }
        private int UpdateHitCount(HitCountSettings hitCountSettings, int hitAmount)
        {
            print(hitCountSettings.HitCount);
            if (hitCountSettings.HitCount <= 0) return 0;
            
            hitCountSettings.HitCount -= hitAmount;
            hitCountSettings.HitCount = Mathf.Clamp(hitCountSettings.HitCount, 0, int.MaxValue);
            return hitCountSettings.HitCount;
        }
        private void HitReaction(Collider selfCollider)
        {
            WhenCollideUlt?.Invoke();

            GenerateHitVFX();

            if (_UseDestroySFX)
                PlayAudio(_NormalAudioIndex);
            else
                PlayRandomAudio();
        }

        private void DestroyReaction(Collider selfCollider)
        {
            if (_UsephPushBackFeedback) PushBackFeedback(selfCollider);

            WhenCollideUlt?.Invoke();

            GenerateHitVFX();

            if (_UseDestroySFX)
                PlayAudio(_DestroyAudioIndex);
            else
                PlayRandomAudio();

            if (TryGetComponent<MoveObject>(out var moveObj))
                moveObj.speed = 0;

            WhenDestroy?.Invoke();

            if (_DestroyAfterHit)
                Destroy(gameObject, _DestroyDelayDuration);
        }
        public void ResetHitCount() {
            AssignHitCountSettings(originalHitCountSettings, _HitCountSettings);
        }
        private void AssignHitCountSettings(List<HitCountSettings> from, List<HitCountSettings> to) {
            for (int i = 0; i < from.Count; i++)
            {
                HitCountSettings hitCount = new HitCountSettings();
                hitCount.InteractType = from[i].InteractType;
                hitCount.HitCount = from[i].HitCount;
                to.Clear();
                to.Add(hitCount);
            }
        }
        private void GenerateHitVFX()
        {
            if (!_GenerateHitVFX) return;

            Project.EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                _HitEffectID,
                false,
                _DestroyDelayDuration,
                transform.position));
        }
        protected override void PushBackFeedback(Collider hitCol)
        {
            base.PushBackFeedback(hitCol);

            var targetPos = transform.position + hitCol.transform.forward * _PushBackDist;
            transform.DOKill();
            transform.DOMove(targetPos, _PushBackDuration).SetEase(_PushBackEasingCureve);
        }
    }
}
