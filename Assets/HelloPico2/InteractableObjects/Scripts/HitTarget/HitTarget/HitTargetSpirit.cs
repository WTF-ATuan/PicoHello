using System;
using System.Collections;
using Project;
using UltEvents;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{    
    public class HitTargetSpirit : HitTargetBase
    {
        public string _BulletReactTimelineName;
        public string _WhipReactTimelineName;
        public string _BeamReactTimelineName;
        public string _EyeTrackingTimelineName;
        [SerializeField] private float _DestroyDelayDuration = 3;
        [SerializeField] private string _BulletReactHitEffectID = "";
        [SerializeField] private string _WhipReactHitEffectID = "";
        [SerializeField] private string _BeamReactHitEffectID = "";
        public UltEvent WhenCollideWithEnergyBall;
        public UltEvent WhenCollideWithWhip;
        public UltEvent WhenCollideWithBeam;
        
        private SpiritTimeline _SpiritTimeline;
        private SpiritTimeline spiritTimeline { 
            get { 
                if(_SpiritTimeline == null)
                    _SpiritTimeline = FindObjectOfType<SpiritTimeline>();
                return _SpiritTimeline;
            } 
        }
        private Collider col;
        Coroutine process;
        private void Awake()
        {
            col = GetComponent<Collider>();

            if (spiritTimeline == null)
                Debug.Log("HitTargetSpirit couldn't get a spiritTimeline ref.");
        }
        protected override void Start()
        {
            base.Start();           
        }
        public override void OnCollide(InteractType type, Collider selfCollider)
        {
            base.OnCollide(type, selfCollider);
            if(type == InteractType.Eye) OnEyeCollide();
        }

        private void OnEyeCollide(){ }

        private void OnEnable(){
            if(spiritTimeline == null){
                throw new Exception("Missing spiritTimeline reference.");  return; }

            OnEnergyBallInteract += BulletReact;
            OnWhipInteract += WhipReact;
            OnBeamInteract += BeamReact;
        }
        private void OnDisable()
        {
            if (spiritTimeline == null) return;

            OnEnergyBallInteract -= BulletReact;
            OnWhipInteract -= WhipReact;
            OnBeamInteract -= BeamReact;
        }
        private void BulletReact(Collider selfCollider)
        {
            WhenCollideWithEnergyBall?.Invoke();

            var playableDuration = spiritTimeline.ActivateTimeline(_BulletReactTimelineName);

            EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                _BulletReactHitEffectID,
                false,
                _DestroyDelayDuration,
                transform.position));

            PlayRandomAudio();

            if (process != null)
                StopCoroutine(process);

            process = StartCoroutine(ColliderControl(playableDuration));
        }
        private void WhipReact(Collider selfCollider)
        {
            WhenCollideWithWhip?.Invoke();

            var playableDuration = spiritTimeline.ActivateTimeline(_WhipReactTimelineName);

            EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                _WhipReactHitEffectID,
                false,
                _DestroyDelayDuration,
                transform.position));

            PlayRandomAudio();

            if (process != null)
                StopCoroutine(process);

            process = StartCoroutine(ColliderControl(playableDuration));
        }
        private void BeamReact(Collider selfCollider)
        {
            WhenCollideWithBeam?.Invoke(); 

            var playableDuration = spiritTimeline.ActivateTimeline(_BeamReactTimelineName);

            EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                _BeamReactHitEffectID,
                false,
                _DestroyDelayDuration,
                transform.position));

            PlayRandomAudio();

            if (process != null)
                StopCoroutine(process);

            process = StartCoroutine(ColliderControl(playableDuration));
        }
        private IEnumerator ColliderControl(float playableDuration) {
            col.enabled = false;
            yield return new WaitForSeconds(playableDuration + hitCDDuration);
            col.enabled = true;
        }
    }    
}
