using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InputDevice.Scripts;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;

namespace HelloPico2.PlayerController.Arm
{
    [RequireComponent(typeof(ArmLogic))]
    public class EnergyBallBehavior : MonoBehaviour
    {
        public XRController _controller;
        [SerializeField] private GameObject _EnergyBall;
        [SerializeField] private Vector2 _ScaleRange;
        [SerializeField] private float _ScalingSpeed;

        [Header("Projectile Settings")]
        [SerializeField] private GameObject _EnergyProjectile;
        [SerializeField] private float _ShootSpeed;
        [SerializeField] private float _ShootCoolDown;
        [SerializeField] private float _CostEnergy;
        [SerializeField] private float _SpeedBufferDuration;

        [Header("Debug")]
        public bool _Debug;
        public float _AxisSpeed;
        public Vector2 axis;
        public Transform target;

        public Coroutine ShootCoolDownProcess { get; set; }

        [SerializeField] private float _ShapingMultiplier;
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

            armLogic.OnGripUp += ShootChargedProjectile;
            armLogic.OnPadInput += UpdateShape;
            armLogic.OnTriggerDown += ShootEnergyProjectile;

            if (currentEnergyBall == null)
            {
                currentEnergyBall = Instantiate(_EnergyBall, _controller.transform);
                currentEnergyBall.transform.localPosition = Vector3.zero;
            }
        }
        private void OnDisable()
        {
            ArmEventHandler.OnChargeEnergy -= ChargeEnergyBall;
            armLogic.OnEnergyChanged -= UpdateScale;

            armLogic.OnGripUp -= ShootChargedProjectile;
            armLogic.OnPadInput -= UpdateShape;
            armLogic.OnTriggerDown -= ShootEnergyProjectile;
        }
        private void Update(){
            if(_Debug) UpdateShape(axis);
        }
        [Button]
        private void ShootChargedProjectile(ArmData data) {
            if (data.Energy <= 0) return;

            GenerateProjectile(true, currentEnergyBall.transform.localScale.x);
            data.Energy = 0;

        }
        [Button]
        private void ShootEnergyProjectile(ArmData data)
        {
            if (data.Energy <= 0 || ShootCoolDownProcess != null) return;

            data.Energy -= _CostEnergy;
            GenerateProjectile(false);
        }
        private void GenerateProjectile(bool overwriteScale, float scale = 1)
        {
            var clone = Instantiate(_EnergyProjectile, transform.root);
            clone.transform.position = _controller.transform.position;
            clone.transform.forward = _controller.transform.forward;
            if (overwriteScale) clone.transform.localScale *= scale;

            var rigidbody = clone.GetComponent<Rigidbody>();
            //rigidbody.AddForce(_ShootSpeed * clone.transform.forward, ForceMode.VelocityChange);

            clone.GetComponent<ProjectileController>().ProjectileSetUp(_ShootSpeed, _SpeedBufferDuration, target);

            Destroy(clone.gameObject, 10);
            
            ShootCoolDownProcess = StartCoroutine(CoolDown(_ShootCoolDown));
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
        private void UpdateShape(Vector2 axis) {
            var scale = currentEnergyBall.transform.localScale;
            scale.x = 1 + scale.x * axis.x * _ShapingMultiplier;
            scale.z = 1 + scale.z * axis.y * _ShapingMultiplier;
            currentEnergyBall.transform.localScale = scale;
            //print(currentEnergyBall.transform.localScale);
        }
        private IEnumerator CoolDown (float duration) {
            yield return new WaitForSeconds(duration);
            StopCoroutine(ShootCoolDownProcess);
            ShootCoolDownProcess = null;
        }
    }
}
