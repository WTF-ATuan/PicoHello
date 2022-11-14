using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class AnimatorSettingsRandomizer : MonoBehaviour
    {
        public bool _RunOnEnable = false;
        [SerializeField] private Animator _Animator;
        [SerializeField] private Vector2 _AnimationSpeed = Vector2.one;

        public bool _UseDelayAnimator = false;
        [ShowIf("_UseDelayAnimator")][SerializeField] private Vector2 _DelayPlayAnimationDuration = Vector2.zero; 

        [ReadOnly][SerializeField] private AnimationClip[] animationClips;
        private void OnEnable()
        {
            animationClips = _Animator.runtimeAnimatorController.animationClips;

            if(_RunOnEnable) SetUpAnimator();
            if(_UseDelayAnimator) StartCoroutine(SeqDelayPlayAnimation());
        }
        public void SetUpAnimator() {
            var speed = Random.Range(_AnimationSpeed.x, _AnimationSpeed.y);
            _Animator.speed = speed;
        }
        private IEnumerator SeqDelayPlayAnimation() {
            var duration = Random.Range(_DelayPlayAnimationDuration.x, _DelayPlayAnimationDuration.y);
            _Animator.enabled = false;
            yield return new WaitForSeconds(duration);
            _Animator.enabled = true;
        }
    }
}
