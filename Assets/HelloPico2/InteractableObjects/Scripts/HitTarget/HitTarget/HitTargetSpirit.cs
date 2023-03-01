using System;
using Sirenix.OdinInspector;
using System.Collections;
using Project;
using UltEvents;
using UnityEngine;
using Sirenix.Utilities;

namespace HelloPico2.InteractableObjects
{    
    public class HitTargetSpirit : HitTargetBase
    {
        public string _BulletReactTimelineName;
        public string _WhipReactTimelineName;
        public string _BeamReactTimelineName;
        [SerializeField] private int _LifePoint = 3;
        [SerializeField] private float _DestroyDelayDuration = 3;
        [SerializeField] private string _BulletReactHitEffectID = "";
        [SerializeField] private string _WhipReactHitEffectID = "";
        [SerializeField] private string _BeamReactHitEffectID = "";
        public UltEvent WhenCollideWithEnergyBall;
        public UltEvent WhenCollideWithWhip;
        public UltEvent WhenCollideWithBeam;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private RandomGuideScript _RandomGuideScript;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private int _EvilSpiritSummonCounter = 3;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private GameObject _EvilSpirit;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private float _CounterResetTime = 99;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private string _SummonEvilTimelineName = "";
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private string _SummonEvilEffectID = "";
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private string _SummonEvilSoundID = "";

        private int currentLifePoint;// { get; set; }
        private bool EvilSummoned;// { get; set; }
        [SerializeField]private int currentEvilCounter;// { get; set; }
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
            currentLifePoint = _LifePoint;
            col = GetComponent<Collider>();

            if (spiritTimeline == null)
                Debug.Log("HitTargetSpirit couldn't get a spiritTimeline ref.");
        }
        private void OnEnable(){
            if(spiritTimeline == null){
                Debug.Log("Missing spiritTimeline reference.");}

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
            GeneralReaction(_BulletReactHitEffectID, _BulletReactTimelineName);
        }
        private void WhipReact(Collider selfCollider)
        {
            WhenCollideWithWhip?.Invoke();
            GeneralReaction(_WhipReactHitEffectID, _BulletReactTimelineName);
        }
        private void BeamReact(Collider selfCollider)
        {
            WhenCollideWithBeam?.Invoke();
            GeneralReaction(_BeamReactHitEffectID, _BulletReactTimelineName);
        }
        private void GeneralReaction(string VFX_ID, string timelineName) {
            if (!EvilSummoned) { 
                if (UpdateEvilCounter()) {
                    VFX_ID = _SummonEvilEffectID;
                    timelineName = _SummonEvilTimelineName;
                } 
            }

            EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                    VFX_ID,
                    false,
                    _DestroyDelayDuration,
                    transform.position));

            PlayRandomAudio();
                      
            if (process != null)
                StopCoroutine(process);

            UpdateLifePoint(-1);            

            if (currentLifePoint > 0) { process = StartCoroutine(ColliderControl(0)); return; }
            else currentLifePoint = _LifePoint;

            var playableDuration = (spiritTimeline != null) ? spiritTimeline.ActivateTimeline(timelineName) : 0;

            process = StartCoroutine(ColliderControl(playableDuration));

            print("Play spirit timeline");
        }
        private void UpdateLifePoint(int point) { 
            currentLifePoint += point;
            currentLifePoint = Mathf.Clamp(currentLifePoint, 0, _LifePoint);
        }
        [Button]
        private bool UpdateEvilCounter()
        {
            if (EvilSummoned) return false;
            
            currentEvilCounter++;
            currentEvilCounter = Mathf.Clamp(currentEvilCounter, 0, _EvilSpiritSummonCounter);

            if (currentEvilCounter < _EvilSpiritSummonCounter)            
                return false;            
            else
            {
                SummonEvil();
                return true;
            }
        }
        private void SummonEvil() {
            print("Summon"); 
            EvilSummoned = true;
            _RandomGuideScript.GuideList.ForEach((x) => x.SetActive(false));
            _EvilSpirit.SetActive(true);

            EventBus.Post(new AudioEventRequested(_SummonEvilSoundID, transform.position));
        }
        private IEnumerator ColliderControl(float playableDuration) {
            col.enabled = false;
            yield return new WaitForSeconds(playableDuration + hitCDDuration);
            col.enabled = true;
        }
    }    
}
