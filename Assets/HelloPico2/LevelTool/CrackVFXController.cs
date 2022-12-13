using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HelloPico2.LevelTool
{
    public class CrackVFXController : MonoBehaviour
    {
        public ParticleSystem _CrackVFX;
        public Transform _SpawnPOS;
        public float _Scale;
        public int _Max;
        public List<ParticleSystem> _SpawnedList = new List<ParticleSystem>();
        public float _ScalingDuration = .3f;
        public AnimationCurve _ScalingEase;

        public void GenerateVFX() {
            if (_SpawnedList.Count > _Max) return;

            var clone = Instantiate(_CrackVFX, _SpawnPOS);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localScale *= _Scale;

            clone.gameObject.SetActive(true);
            clone.Play(true);

            clone.transform.DOScale(clone.transform.localScale, _ScalingDuration).From(0).SetEase(_ScalingEase);
            
            _SpawnedList.Add(clone);
        }
        public void ClearEffect() {
            foreach (var obj in _SpawnedList)
                Destroy(obj);

            _SpawnedList.Clear();
        }
    }
}
