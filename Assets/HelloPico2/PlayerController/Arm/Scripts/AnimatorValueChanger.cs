using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;
using DG.Tweening;

namespace HelloPico2.PlayerController.Arm
{    
    [RequireComponent(typeof(Animator))]
    public class AnimatorValueChanger : MonoBehaviour
    {
        public Animator animator { get; private set; }
        private void Start()
        {
            animator = GetComponent<Animator>();
        }
        public void LerpFloat(string name, float duration, float to) {
            var floatValue = animator.GetFloat(name);
            DOTween.To(() => floatValue, x => floatValue = x, to, duration).OnUpdate(() => { animator.SetFloat(name, floatValue); });
        }
    }
}
