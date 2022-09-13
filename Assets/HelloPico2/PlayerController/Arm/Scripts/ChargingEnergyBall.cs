using HelloPico2.InputDevice.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm
{
    public class ChargingEnergyBall : MonoBehaviour, IGainEnergyFeedback, IShootingFeedback
    {
        public Shooting _EnergyBallDeformEffect;

        Coroutine process;
        public void OnNotify(HandType hand)
        {
            if (process != null)
            { 
                _EnergyBallDeformEffect.enabled = false;
                StopCoroutine(process); 
            }

            process = StartCoroutine(Delayer());
        }
        private IEnumerator Delayer() {
            _EnergyBallDeformEffect.enabled = true;
            yield return new WaitForSeconds(0.6f);
            _EnergyBallDeformEffect.enabled = false;
        }
    }
}
