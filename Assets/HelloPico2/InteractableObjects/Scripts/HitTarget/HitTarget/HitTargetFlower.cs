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
        [SerializeField] private GameObject _SpawnThisWhenBloomed;
        [SerializeField] private int _SpawnAmount;
        [SerializeField] private FollowParticle _SpawnObjectControl;
        [SerializeField] private ParticleSystem _FollowVFXPosition;

        public UnityEvent WhenCharged1;
        public UnityEvent WhenCharged2;
        public UnityEvent WhenFullyCharged;

        private bool charged1;
        private bool charged2;
        private bool bloomed;
        private float currentChargedEnergy;

        public override void OnCollide(InteractType type, Collider selfCollider)
        {
            WhenCollide?.Invoke();
            base.OnCollide(type, selfCollider);
        }
        private void OnEnable()
        {
            OnEnergyBallInteract += ChargeBloom;          
        }
        private void OnDisable()
        {
            OnEnergyBallInteract -= ChargeBloom;          
        }
        private void ChargeBloom(Collider selfCollider) {   
            if (bloomed) return;

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
                SpawnEnergy();
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
            for (int i = 0; i < _SpawnAmount; i++)
            {
                var clone = Instantiate(_SpawnThisWhenBloomed, transform.position, Quaternion.identity);
                clone.transform.SetParent(transform.parent, false);                
                cloneList.Add(clone);
            }

            _SpawnObjectControl.m_Follower = cloneList.ToArray();
            _SpawnObjectControl.m_FollowThis = _FollowVFXPosition;
            _SpawnObjectControl.Activate = true;
            _FollowVFXPosition.Play();
        }
    }
}
