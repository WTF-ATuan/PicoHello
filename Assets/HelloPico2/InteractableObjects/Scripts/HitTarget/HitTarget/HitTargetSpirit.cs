using System;
using Sirenix.OdinInspector;
using System.Collections;
using Project;
using UltEvents;
using UnityEngine;
using Sirenix.Utilities;
using UnityEditor;
using DG.Tweening;

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
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private EmissionRaiseSteps _TeasingColorControl;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private int _EvilSpiritTeasingCounter = 3;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private int _EvilSpiritSummonCounter = 8;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private float _GlowCurrentSpiritDuration = 1.2f;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private float _TransformDuration = .5f;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private float _GlowEvilSpiritDuration = .5f;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private GameObject _EvilSpirit;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private float _CounterResetTime = 99;
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private string _SummonEvilTimelineName = "";
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private string _SummonEvilEffectID = "";
        [FoldoutGroup("Evil Spirit Settings")][SerializeField] private string _TeasingEvilSoundID = "";
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
        private bool _Teased;

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

            if (!EvilSummoned)
            {
                if (UpdateEvilCounter())
                {
                    timelineName = _SummonEvilTimelineName;
                }
            }

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

            if (currentEvilCounter == _EvilSpiritTeasingCounter && !_Teased)
            {
                _Teased = true;
                
                _RandomGuideScript.GuideList.ForEach((guide) => { 
                    if (guide.activeSelf == true) {
                        _TeasingColorControl.TargetRenderer = guide.GetComponents<Renderer>();
                        _TeasingColorControl.RaiseToValue(1);

                    } 
                });
                
                EventBus.Post(new AudioEventRequested(_TeasingEvilSoundID, transform.position));
            }
                
            if (currentEvilCounter < _EvilSpiritSummonCounter)            
                return false;            
            else
            {
                SummonEvil();
                return true;
            }
        }
        private void SummonEvil() {
            //print("Summon"); 
            EvilSummoned = true;

            GlowEvilSpiritSeq();
        }
        private void GlowEvilSpiritSeq() {
            Sequence seq = DOTween.Sequence();

            TweenCallback glowCurrent = () => { 
                _TeasingColorControl.m_ControlValueName = "_GradientUVAdd";
                _TeasingColorControl.RaiseToValue(1, false, true, _GlowCurrentSpiritDuration * 0.6f);
            };
            TweenCallback glowEvil = () => {

                EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                        _SummonEvilEffectID,
                        false,
                        _DestroyDelayDuration,
                        transform.position));

                EventBus.Post(new AudioEventRequested(_SummonEvilSoundID, transform.position));

                _EvilSpirit.SetActive(true);
                _TeasingColorControl.m_ControlValueName = "_SelfShadowOffSet";
                _TeasingColorControl.TargetRenderer = _EvilSpirit.GetComponents<Renderer>();
                _TeasingColorControl.SetColor(1);
                _TeasingColorControl.SetValue(2);
            };
            TweenCallback switchSpirit = () => {
                _RandomGuideScript.GuideList.ForEach((x) => x.SetActive(false));
                _TeasingColorControl.RaiseToValue(3,false,false, _GlowEvilSpiritDuration * 0.6f);
            }; 
            TweenCallback endTransformSeq = () => {
                _TeasingColorControl.RaiseToColor(0, false, false, 1);
            };

            seq.AppendCallback(glowCurrent);
            seq.AppendInterval(_GlowCurrentSpiritDuration);
            seq.AppendCallback(glowEvil);
            seq.AppendInterval(_TransformDuration);
            seq.AppendCallback(switchSpirit);
            seq.AppendInterval(_GlowEvilSpiritDuration);
            seq.AppendCallback(endTransformSeq);

            seq.Play();
        }
        private IEnumerator ColliderControl(float playableDuration) {
            col.enabled = false;
            yield return new WaitForSeconds(playableDuration + hitCDDuration);
            col.enabled = true;
        }
    }    
}
