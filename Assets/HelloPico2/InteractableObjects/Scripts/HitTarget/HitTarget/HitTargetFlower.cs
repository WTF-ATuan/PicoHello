using System.Collections;
using System.Collections.Generic;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetFlower : HitTargetBase
    {
        [FoldoutGroup("Audio Settings")][SerializeField] private string _ChargeBloomClipName0;
        [FoldoutGroup("Audio Settings")][SerializeField] private string _ChargeBloomClipName1;
        [SerializeField] private float _RequireEnergy;
        [FoldoutGroup("Spawn Energy")][SerializeField] private GameObject _SpawnThisWhenBloomed;
        [FoldoutGroup("Spawn Energy")][SerializeField] private bool _SpawnEnergyFromAnimationEvent = false;
        [FoldoutGroup("Spawn Energy")][SerializeField] private int _SpawnAmount;
        [FoldoutGroup("Spawn Energy")][SerializeField] private float _DelayToActivate = .5f;
        [FoldoutGroup("Spawn Energy")][SerializeField] private AnimationCurve _SpawnedEnergyEasingCurve;
        [FoldoutGroup("Spawn Energy")][SerializeField] private FollowParticle _SpawnObjectControl;
        [FoldoutGroup("Spawn Energy")][SerializeField] private ParticleSystem _FollowVFXPosition;
        [FoldoutGroup("Spawn Energy")][SerializeField] private float _DelayToDestroy = 3f;        

        public UltEvents.UltEvent WhenCharge;
        public UnityEvent WhenCharged1;
        public UnityEvent WhenCharged2;
        public UnityEvent WhenFullyCharged;
        public UnityEvent WhenFinishedBloom;

        private bool charged1;
        private bool charged2;
        private bool bloomed;
        private float currentChargedEnergy;
        public HelloPico2.InputDevice.Scripts.DeviceInputDetected _deviceInput { get; set; }

        public override void OnCollide(InteractType type, Collider selfCollider){
            base.OnCollide(type, selfCollider);
            if (selfCollider.TryGetComponent<HelloPico2.PlayerController.Arm.ProjectileController>(out var projectileController))
                _deviceInput = projectileController._deviceInput;
        }
        private void OnEnable(){
            OnEnergyBallInteract += ChargeBloom;          
            OnBeamInteract += PlayHitEffect;          
            OnWhipInteract += PlayHitEffect;              
            OnShieldInteract += PlayHitEffect;              
            OnEnergyInteract += PlayHitEffect;              
        }
        private void OnDisable(){
            OnEnergyBallInteract -= ChargeBloom;
            OnBeamInteract -= PlayHitEffect;
            OnWhipInteract -= PlayHitEffect;
            OnShieldInteract -= PlayHitEffect;
            OnEnergyInteract -= PlayHitEffect;
        }
        private void PlayHitEffect(Collider selfCollider){
            if (!_timer.CanInvoke()) return;
            if (bloomed) return;

            WhenCollide?.Invoke();
            WhenCollideUlt?.Invoke();
            if (_UsephPushBackFeedback) PushBackFeedback(selfCollider);
        }
        private void ChargeBloom(Collider selfCollider) {
            if (!_timer.CanInvoke()) return;
            if (bloomed) return;

            WhenCharge?.Invoke();
            if (_UsephPushBackFeedback) PushBackFeedback(selfCollider);

            currentChargedEnergy += 20;
            CheckChargedEnergy();

            PlayAudio();
        }
        private void CheckChargedEnergy() {
            if (currentChargedEnergy >= _RequireEnergy / 3 && !charged1)
            {
                WhenCharged1?.Invoke();
                charged1 = true;
            }
            if (currentChargedEnergy >= _RequireEnergy * 2 / 3 && !charged2)
            {
                WhenCharged2?.Invoke();
                charged2 = true;
            }
            if (currentChargedEnergy >= _RequireEnergy && !bloomed) 
            { 
                WhenFullyCharged?.Invoke();
                if(!_SpawnEnergyFromAnimationEvent) SpawnEnergy();
                bloomed = true;
            }
        }
        private void PlayAudio() {
            var value = Random.Range(0, 2);
            string result = "";
            if (value == 0)
                result = _ChargeBloomClipName0;
            else
                result = _ChargeBloomClipName1;

            EventBus.Post(new AudioEventRequested(result, transform.position));
        }
        public void SpawnEnergy() {
            List<GameObject> cloneList = new List<GameObject>();
            var lifeTime =_FollowVFXPosition.main.startLifetime.Evaluate(0);

            for (int i = 0; i < _SpawnAmount; i++)
            {
                var clone = Instantiate(_SpawnThisWhenBloomed, _FollowVFXPosition.transform.position, Quaternion.identity);
                clone.transform.SetParent(transform.parent);
                clone.transform.position = _FollowVFXPosition.transform.position;
                cloneList.Add(clone);

                var Seq = DOTween.Sequence();
                Seq.Append(clone.transform.DOScale(clone.transform.localScale, _DelayToActivate).From(Vector3.zero).SetEase(_SpawnedEnergyEasingCurve));
                Seq.AppendInterval(lifeTime).OnComplete(() => {
                    if (clone.TryGetComponent<InteractablePower>(out var interactablePower))
                    {
                        if (_deviceInput != null)
                            interactablePower?.OnSelect(_deviceInput);
                    }
                });
                Seq.Play();
            }

            StartCoroutine(EnergyActivition(cloneList));

            _SpawnObjectControl.m_Follower = cloneList.ToArray();
            _SpawnObjectControl.m_FollowThis = _FollowVFXPosition;
            _SpawnObjectControl.Activate = true;
            _SpawnObjectControl.WhenParticleDies = delegate(GameObject obj) {
                //if (obj.TryGetComponent<InteractablePower>(out var interactablePower))
                //{
                //    if (_deviceInput != null)
                //        interactablePower?.OnSelect(_deviceInput);
                //}
            };
            _FollowVFXPosition.Play();
        }
        private IEnumerator EnergyActivition(List<GameObject> objs) {            
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].GetComponent<Collider>().enabled = false;
            }
            yield return new WaitForSeconds(_DelayToActivate);
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].GetComponent<Collider>().enabled = true;
            }
            WhenFinishedBloom?.Invoke(); 
            transform.DOScale(Vector3.zero, _DelayToDestroy);
            Destroy(gameObject, _DelayToDestroy);
        }
        protected override void PushBackFeedback(Collider hitCol)
        {
            base.PushBackFeedback(hitCol);

            var targetPos = transform.position + hitCol.transform.forward * _PushBackDist;

            transform.DOMove(targetPos, _PushBackDuration).SetEase(_PushBackEasingCureve);
        }
        
    }
}
