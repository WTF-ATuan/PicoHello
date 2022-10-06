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
        public void OnHit(InteractType type) {
            _HitReactionEventWithoutCD?.Invoke();

            UpdateHitCount(type);

            if (currentHitCount < _TriggerHitReactionHitCount) return;

            if(_HitAudioClipName != null)
                AudioPlayerHelper.PlayAudio(_HitAudioClipName, transform.position);

            _HitReactionEvent?.Invoke();
        }
        private void UpdateHitCount(InteractType type) {
            if (currentHitCount < _TriggerHitReactionHitCount)
            {
                currentHitCount++;

                switch (type)
                {
                    case InteractType.Beam:
                        break;
                    case InteractType.Whip:
                        break;
                    case InteractType.EnergyBall:
                        break;
                    case InteractType.Shield:
                        break;
                    case InteractType.Energy:
                        break;
                    case InteractType.Eye:
                        break;
                    case InteractType.Bomb:
                        currentHitCount = _TriggerHitReactionHitCount;
                        break;
                    default:
                        break;
                }
            }
            else
                currentHitCount = 0;
        }        
    }
}
