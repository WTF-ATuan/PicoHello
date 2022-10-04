using System.Collections;
using System.Collections.Generic;
using Game.Project;
using UltEvents;
using UnityEngine;

namespace HelloPico2.InteractableObjects.Scripts
{
    public class BossVisualReaction : MonoBehaviour
    {
        [SerializeField] private float _HitReactionCD = 2f;
        [SerializeField] private string _HitAudioClipName;
        ColdDownTimer coldDownTimer;

        public UltEvent _HitReactionEvent;
        private void Awake()
        {
            coldDownTimer = new ColdDownTimer(_HitReactionCD);
        }
        public void OnHit() {
            if (!coldDownTimer.CanInvoke()) return;

            if(_HitAudioClipName != null)
                AudioPlayerHelper.PlayAudio(_HitAudioClipName, transform.position);

            _HitReactionEvent?.Invoke();

            coldDownTimer.Reset();        
        }
    }
}
