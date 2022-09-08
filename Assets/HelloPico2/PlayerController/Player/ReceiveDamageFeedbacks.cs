using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace HelloPico2.LevelTool
{
    public class ReceiveDamageFeedbacks : MonoBehaviour
    {
        public Image _DamageVignetteImage;
        public float _ToAlpha = 0.3f;
        public float _StayDuration = 3;
        public float _Duration = 3;

        public void PlayDamageFeedbacks() {
            Sequence seq = DOTween.Sequence();
            seq.Append(_DamageVignetteImage.DOFade(_ToAlpha, _Duration));
            seq.AppendInterval(_StayDuration);
            seq.Append(_DamageVignetteImage.DOFade(0, _Duration));

            seq.Play();
        }
    }
}
