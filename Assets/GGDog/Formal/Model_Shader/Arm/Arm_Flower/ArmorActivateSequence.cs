using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

        Coroutine process;
        public Material[] originalMat;

        private void OnEnable()
        {
            if(_ArmorController)
                _ArmorController.WhenActivateArmor += ActivateTargetArmor;
        }
        private void OnDisable()
        {
            if(_ArmorController)
                _ArmorController.WhenActivateArmor -= ActivateTargetArmor;            
        }
        private void ActivateTargetArmor(GameObject armor) {
            _Armor = armor.GetComponent<Renderer>();
            ActivateArmor();
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
            Sequence seq = DOTween.Sequence();
            seq.Append(_Armor.materials[1].DOFloat(_ToRim, _RimStrengthName, _RimEffectDuration));
            seq.AppendInterval(_RimEffectStayDuration);
            seq.Append(_Armor.materials[1].DOFloat(_FromRim, _RimStrengthName, _RimEffectDuration));
            TweenCallback SeqCompleteCallback = delegate { _Armor.materials = originalMat; };
            seq.AppendCallback(SeqCompleteCallback);

            // Fade
            _Armor.materials[0].SetColor(_FadeColorName, _FromColor);
            Sequence seq1 = DOTween.Sequence();
            seq1.Append(_Armor.materials[0].DOColor(_FromColor, _FadeColorName, .01f));
            seq1.AppendInterval(_FadeInDelay);
            seq1.Append(_Armor.materials[0].DOColor(_ToColor, _FadeColorName, _FadeInDuration).From(_FromColor));
            seq1.AppendInterval(_FadeInStay); 
            // Fix it
            //TweenCallback Seq1CompleteCallback = delegate { _Armor.materials[2] = originalMat[0]; };
            List<Material> mats = new List<Material>(_Armor.materials);
            mats.Add(originalMat[0]);
            TweenCallback Seq1CompleteCallback = delegate { _Armor.materials = mats.ToArray(); };
            seq1.AppendCallback(Seq1CompleteCallback);
            seq1.Append(_Armor.materials[0].DOColor(_FromColor, _FadeColorName, _FadeInDuration).From(_ToColor));
            
            seq.Play();
            seq1.Play();
        }
    }
}
