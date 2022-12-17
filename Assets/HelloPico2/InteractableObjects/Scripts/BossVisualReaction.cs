using System;
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
        [SerializeField] private ParticleSystem _CrackVFX;
        [SerializeField] private ParticleSystem[] _EnergyHitVFX;
        [SerializeField] private float _EnergyHitVFXCD = 0.5F;
        
        int currentHitCount;        
        Game.Project.ColdDownTimer coolDownTimer;
        Coroutine HitVFXProcess;

        public UltEvent _HitReactionEvent;
        public UltEvent _HitReactionEventWithoutCD;
        private void OnEnable()
        {
            coolDownTimer = new ColdDownTimer(_EnergyHitVFXCD);
        }
        public void OnHit(InteractType type, Vector3 collisionPoint) {
            _HitReactionEventWithoutCD?.Invoke();
            PlayCrackVFX(collisionPoint);
            UpdateHitCount(type);

            if (coolDownTimer.CanInvoke())
            { 
                PlayHitVFX(type); 
                coolDownTimer.Reset();
            }

            if (currentHitCount < _TriggerHitReactionHitCount) return;            

            if (_HitAudioClipName != null)
                AudioPlayerHelper.PlayAudio(_HitAudioClipName, transform.position);

            _HitReactionEvent?.Invoke();
        }

        private void PlayCrackVFX(Vector3 collisionPoint)
        {
            _CrackVFX.Stop();
            _CrackVFX.transform.position = collisionPoint;
            _CrackVFX.Play(true);
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
        private void PlayHitVFX(InteractType type) {
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
                    if (_EnergyHitVFX.Length != 0)
                    {
                        for (int i = 0; i < _EnergyHitVFX.Length; i++)
                            _EnergyHitVFX[i].Play();

                        if(HitVFXProcess != null)
                            StopCoroutine(HitVFXProcess);

                        HitVFXProcess = StartCoroutine(HitVFXSwitcher(_EnergyHitVFX));
                    }
                    break;
                case InteractType.Eye:
                    break;
                case InteractType.Bomb:
                    break;
                default:
                    break;
            }
        }
        private IEnumerator HitVFXSwitcher(ParticleSystem[] VFX) {
            yield return new WaitForSeconds(_EnergyHitVFXCD);
            for (int i = 0; i < VFX.Length; i++)
                VFX[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
