using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.PlayerController.Arm
{
    [RequireComponent(typeof(ArmLogic))]
    public class EnergyBallBehavior : MonoBehaviour
    {
        public XRController _controller;
        [SerializeField] private GameObject _EnergyBall;
        [SerializeField] private Vector2 _ScaleRange;
        [SerializeField] private float _ScalingSpeed;

        private ArmLogic _ArmLogic;
        ArmLogic armLogic { 
            get { 
                if (_ArmLogic == null) 
                    _ArmLogic = GetComponent<ArmLogic>(); 

                return _ArmLogic;
            } 
        }

        private GameObject currentEnergyBall;
       
        private void OnEnable()
        {
            ArmEventHandler.OnChargeEnergy += ChargeEnergyBall;
            armLogic.OnEnergyChanged += UpdateScale;
        }
        private void OnDisable()
        {
            ArmEventHandler.OnChargeEnergy -= ChargeEnergyBall;
            armLogic.OnEnergyChanged -= UpdateScale;
        }
        private void ChargeEnergyBall(float amount, IXRSelectInteractable interactable, DeviceInputDetected obj) {
            // Check same arm

            if (currentEnergyBall == null)
            {
                currentEnergyBall = Instantiate(_EnergyBall, obj.Selector.SelectorTransform);
                currentEnergyBall.transform.localPosition = Vector3.zero;
            }
        }
        private void UpdateScale(ArmData data) {
            currentEnergyBall.transform.localScale = Vector3.one * Mathf.Lerp(_ScaleRange.x, _ScaleRange.y, data.Energy / data.MaxEnergy) ;
        }
    }
}
