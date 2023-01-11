using HelloPico2.InputDevice.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HelloPico2.PlayerController.Arm
{
    public class ChargingEnergyBall : MonoBehaviour, IActivationNotify, IGainEnergyFeedback, IShootingFeedback, IFullEnergyFeedback
    {
        public Shooting _EnergyBallDeformEffect;
        public float _Punch = 1.3f;
        public float _PunchDuration = 0.3f;
        public int _Vibrato = 5;
        public GameObject _Mesh;
        public GameObject _BadgeVFX;
        public float _BadgeTurnOnDelayDuration = 0.1f;
        public Follower _Follower;
        public UltEvents.UltEvent _WhenChargeEnergy;
        public UltEvents.UltEvent _WhenFullyCharged;
        public UltEvents.UltEvent _WhenExitFullyCharged;
        public UltEvents.UltEvent _WhenSwitchToSwordOrShield;
        public UltEvents.UltEvent _WhenSwitchToEnergyball;
        private Sequence punchSeq;

        private Renderer _MeshRenderer;
        private Renderer meshRenderer { 
            get { 
                if (_MeshRenderer == null)
                    _MeshRenderer = _Mesh.GetComponent<Renderer>();
                return _MeshRenderer;
            }
        }
        bool isCharged { get; set; }
        Coroutine process;
        
        public void ExitFullEnergyNotify(HandType hand)
        {
            if (!isCharged) return;

            isCharged = false;
            _WhenExitFullyCharged?.Invoke();
        }

        public void OnFullEnergyNotify(HandType hand)
        {
            if (isCharged) return;
            
            isCharged = true;
            _WhenFullyCharged?.Invoke();
        }
        public void OnNotify(HandType hand)
        {
            if (process != null) return;

            process = StartCoroutine(Delayer());

            PunchScale();

            _WhenChargeEnergy?.Invoke();
        }
        public void OnNotify(HelloPico2.PlayerController.Arm.ArmData armData, HelloPico2.PlayerController.Arm.EnergyBallBehavior energyBallBehavior)
        {
            if (!energyBallBehavior.isCurrentWeaponEnergyBall() || energyBallBehavior.hasTransformProcess) return;

            if (armData.Energy <= 0)
            {
                meshRenderer.GetComponent<Follower>().m_Activation = false;
                meshRenderer.enabled = false;
                return;
            }
            else
            {
                meshRenderer.GetComponent<Follower>().m_Activation = true;
                meshRenderer.enabled = true;
            }

            PunchScale();

            // TOFIX: Process will always have value 
            if (process == null && gameObject.activeSelf)
            {
                process = StartCoroutine(Delayer());
            }

            _WhenChargeEnergy?.Invoke();            
        }
        public void PunchScale()
        {
            if (process != null) return;

            _Follower.m_Scale = false;
            punchSeq.Append(
            _Mesh.transform.DOPunchScale(_Mesh.transform.localScale * _Punch, _PunchDuration, _Vibrato).OnComplete(() => {
                _Follower.m_Scale = true;
            })
            );
            punchSeq.Play();
        }
        private IEnumerator Delayer() {
            yield return new WaitForSeconds(_PunchDuration);
            process = null;
        }

        public void OnNotifyActivate()
        {
            //print("isCharged " + isCharged);
            if (isCharged)
            {
                //print("Notify Activate");
                _WhenFullyCharged?.Invoke();                    
            }

            _WhenSwitchToEnergyball?.InvokeSafe();

            if (_BadgeVFX == null) return;
            SwitchBadge(true);
        }

        public void OnNotifyDeactivate()
        {
            //print("isCharged " + isCharged);
            if (isCharged)
            {
                //print("Notify Deactivate");
                _WhenExitFullyCharged?.Invoke();                
            }

            _WhenSwitchToSwordOrShield?.InvokeSafe();

            if (_BadgeVFX == null) return;
            SwitchBadge(false);
        }
        private void SwitchBadge(bool value) { 
            _BadgeVFX.SetActive(value);
        }
        private void OnDestroy()
        {
            Destroy(_Mesh);
        }
    }
}
