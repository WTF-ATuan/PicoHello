using HelloPico2.InputDevice.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HelloPico2.PlayerController.Arm
{
    public class ChargingEnergyBall : MonoBehaviour, IGainEnergyFeedback, IShootingFeedback, IFullEnergyFeedback
    {
        public Shooting _EnergyBallDeformEffect;
        public float _Punch = 1.3f;
        public float _PunchDuration = 0.3f;
        public int _Vibrato = 5;
        public GameObject _Mesh;
        public Follower _Follower;
        public UltEvents.UltEvent _WhenChargeEnergy;
        public UltEvents.UltEvent _WhenFullyCharged;

        Coroutine process;

        public void OnFullEnergyNotify(HandType hand)
        {
            _WhenFullyCharged?.Invoke();
        }

        public void OnNotify(HandType hand)
        {
            if (process != null) return;                

            process = StartCoroutine(Delayer());

            PunchScale();
            
            _WhenChargeEnergy?.Invoke();
        }
        public void PunchScale() {
            _Mesh.transform.DOPunchScale(_Mesh.transform.localScale * _Punch, _PunchDuration, _Vibrato);
        }
        private IEnumerator Delayer() {
            _Follower.m_Scale = false; 
            yield return new WaitForSeconds(_PunchDuration);
            _Follower.m_Scale = true;
            process = null;
        }
    }
}
