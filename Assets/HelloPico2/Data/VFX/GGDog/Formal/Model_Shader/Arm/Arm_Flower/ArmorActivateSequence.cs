using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HelloPico2.Singleton;

namespace HelloPico2.PlayerController.Arm
{
    public class ArmorActivateSequence : MonoBehaviour
    {
        public ArmorController _ArmorController;
        public Renderer _Armor;
        [Header("Rim")]
        public Material _Rim;
        public string _RimStrengthName = "RimStrength";
        public float _FromRim = 0;
        public float _ToRim = 1;
        public float _RimEffectStayDuration;
        public float _RimEffectDuration;
        [Header("Fade In")]
        public Material _Fade;
        public string _FadeColorName = "";
        public Color _FromColor;
        public Color _ToColor;
        public float _FadeInDelay;
        public float _FadeInStay;
        public float _FadeInDuration;

        [Header("VFX")]
        public ParticleSystem _BubbleVFX;
        public ParticleSystem _RoundingVFX;
        public GameObject _AnimationEffect;
        public HelloPico2.LevelTool.SkinnedMeshEffectPlacement _EffectPlacement;

        [Header("Audio Settings")]
        [SerializeField] private string _ShowArmorClipName = "ShowArmor";

        Coroutine process;
        public Material[] originalMat;

        private Sequence seq;
        private Sequence seq1;

        private void OnEnable()
        {
            if (_ArmorController)
            {
                _ArmorController.WhenActivateArmor += ActivateTargetArmor;
                _ArmorController.WhenActivateArmor += HelloPico2.Singleton.ArmorUpgradeSequence.Instance.UpdatePlayerArmorPosition;
            }
        }
        private void OnDisable()
        {
            if (_ArmorController)
            {
                _ArmorController.WhenActivateArmor -= ActivateTargetArmor;
                if(ArmorUpgradeSequence.Instance){
                    _ArmorController.WhenActivateArmor -= HelloPico2.Singleton.ArmorUpgradeSequence.Instance.UpdatePlayerArmorPosition;
                }
            }
        }
        private void ActivateTargetArmor(GameObject armor) {
            //_Armor = armor.GetComponent<Renderer>();
            //ActivateArmor();
            QueueActivation(armor);
        }
        List<GameObject> queueList = new List<GameObject>();
        private void QueueActivation(GameObject armor) {             
            queueList.Add(armor);
            armor.SetActive(false);

            if (process == null)
                process = StartCoroutine(ActivateArmorprocess());
        }
        private IEnumerator ActivateArmorprocess() {
            yield return new WaitForSeconds(HelloPico2.Singleton.ArmorUpgradeSequence.Instance._AttractorVFXDuration);

            while (queueList.Count > 0) {
                _Armor = queueList[0].GetComponent<Renderer>();
                if (_Armor == null) { yield return null; continue; }
                ActivateArmor();
                yield return new WaitUntil(() => !seq.IsPlaying() && !seq1.IsPlaying());
                queueList.RemoveAt(0);
            }
            queueList.Clear();
            process = null;
        }
        public void ActivateArmor() {
            List<Material> materials = new List<Material>();
            materials.Add(_Fade);
            materials.Add(_Rim);
            materials.Add(_Fade);
            originalMat = _Armor.sharedMaterials;
            _Armor.materials = materials.ToArray();

            // Rim
            _Armor.materials[1].SetFloat(_RimStrengthName, _FromRim);
            seq = DOTween.Sequence();
            seq.Append(_Armor.materials[1].DOFloat(_ToRim, _RimStrengthName, _RimEffectDuration));
            seq.AppendInterval(_RimEffectStayDuration);
            seq.Append(_Armor.materials[1].DOFloat(_FromRim, _RimStrengthName, _RimEffectDuration));
            TweenCallback SeqCompleteCallback = delegate { 
                _Armor.materials = originalMat; 
                TurnOffAnimationEffect();
            };
            seq.AppendCallback(SeqCompleteCallback);

            // Fade
            _Armor.materials[0].SetColor(_FadeColorName, _FromColor);
            seq1 = DOTween.Sequence();
            seq1.Append(_Armor.materials[0].DOColor(_FromColor, _FadeColorName, .01f));
            seq1.AppendInterval(_FadeInDelay);
            seq1.Append(_Armor.materials[0].DOColor(_ToColor, _FadeColorName, _FadeInDuration).From(_FromColor));
            seq1.AppendInterval(_FadeInStay); 
            // Fix it
            //TweenCallback Seq1CompleteCallback = delegate { _Armor.materials[2] = originalMat[0]; };
            List<Material> mats = new List<Material>(_Armor.materials);
            mats.Add(originalMat[0]);
            TweenCallback Seq1CompleteCallback = delegate { 
                _Armor.materials = mats.ToArray(); 
                PlayBubbleVFX();

                // VFX
                var clone = Instantiate(_RoundingVFX, transform);
                _EffectPlacement.SetPosition(_Armor, clone.transform);
                clone.Play();
                print("Play Rounding Effect");
                Destroy(clone, 5);

                GainArmorEvent GainArmroevent = new GainArmorEvent();
                Project.EventBus.Post(GainArmroevent);
            };
            seq1.AppendCallback(Seq1CompleteCallback);
            seq1.Append(_Armor.materials[0].DOColor(_FromColor, _FadeColorName, _FadeInDuration).From(_ToColor));

            _Armor.gameObject.SetActive(true);
            seq.Play();
            seq1.Play();

            // Animation Effect            
            PlayGlowingAnimationEffect();
        }
        private void TurnOffAnimationEffect() {
            _AnimationEffect.SetActive(false);
        }
        private void PlayGlowingAnimationEffect() {
            _EffectPlacement.SetPosition(_Armor, _AnimationEffect.transform);
            _AnimationEffect.SetActive(true);
            AudioPlayerHelper.PlayAudio(_ShowArmorClipName, transform.position);
        }
        private void PlayBubbleVFX() {
            var vfxShape = _BubbleVFX.shape;
            vfxShape.skinnedMeshRenderer = _Armor.GetComponent<SkinnedMeshRenderer>();

            _BubbleVFX.Play();
        }
    }
}
