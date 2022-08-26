using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HelloPico2.LevelTool
{
    public class TrailFader : MonoBehaviour
    {
        public TrailRenderer _Trail;
        private float originalTrailWidthMultiplier;
        private void Start()
        {
            originalTrailWidthMultiplier = _Trail.widthMultiplier;
        }
        public void SetUpTrailWidthMul(float width = 1)
        {
            _Trail.widthMultiplier = width;
        }
        public void FadeOutTrail(float duration = 1) {
            var mul = _Trail.widthMultiplier;

            DOTween.To(() => mul, x => mul = x, 0, duration).OnUpdate(() => { _Trail.widthMultiplier = mul; });
        }
    }
}
