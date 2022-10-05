using System.Collections;
using System.Collections.Generic;
using Game.Project;
using UltEvents;
using UnityEngine;

namespace HelloPico2.InteractableObjects.Scripts
{
    public class BossVisualReaction : MonoBehaviour
    {
        [SerializeField] private int _TriggerHitReactionHitCount = 5;
        [SerializeField] private string _HitAudioClipName;
        int currentHitCount;

        public UltEvent _HitReactionEvent;
        public UltEvent _HitReactionEventWithoutCD;
        public void OnHit() {
            _HitReactionEventWithoutCD?.Invoke();

            UpdateHitCount();

            if (currentHitCount < _TriggerHitReactionHitCount) return;

            if(_HitAudioClipName != null)
                AudioPlayerHelper.PlayAudio(_HitAudioClipName, transform.position);

            _HitReactionEvent?.Invoke();
        }
        private void UpdateHitCount() {
            if (currentHitCount < _TriggerHitReactionHitCount)
                currentHitCount++;
            else
                currentHitCount = 0;
        }        
    }
}
