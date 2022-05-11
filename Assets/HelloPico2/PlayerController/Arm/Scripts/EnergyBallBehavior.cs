using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InputDevice.Scripts;
using HelloPico2.InteractableObjects;
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
        [SerializeField] private float _SpawnOffset;
        [SerializeField] private float _ShootSpeed;
        [SerializeField] private float _ShootCoolDown;
        [SerializeField] private float _CostEnergy;
        [SerializeField] private float _SpeedBufferDuration;

        [Header("Shape Settings")]
        [SerializeField] private GameObject _Sword;
        [SerializeField] private GameObject _Whip;
        [SerializeField] private GameObject _Shield;
        private SwordBehavior swordBehavior;

        [Header("Debug")]
        public bool _Debug;
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
        private GameObject currentShape;
        private bool isShapeConfirmed = false;
       
        private void OnEnable()
        {
            ArmEventHandler.OnChargeEnergy += ChargeEnergyBall;
            armLogic.OnEnergyChanged += UpdateScale;

            armLogic.OnGripUp += ShootChargedProjectile;

            armLogic.OnPrimaryAxisInput += UpdateShape;
            armLogic.OnPrimaryAxisClick += ConfirmShape;
            armLogic.OnPrimaryAxisTouchUp += ExitShapeControlling;

            armLogic.OnTriggerDown += ShootEnergyProjectile;

            if (currentEnergyBall == null)
            {
                currentEnergyBall = Instantiate(_EnergyBall, _controller.transform);
                currentEnergyBall.transform.localPosition = Vector3.zero;
            }
            currentShape = _Sword;
        }
        private void OnDisable()
        {
            ArmEventHandler.OnChargeEnergy -= ChargeEnergyBall;
            armLogic.OnEnergyChanged -= UpdateScale;

            armLogic.OnGripUp -= ShootChargedProjectile;

            armLogic.OnPrimaryAxisInput -= UpdateShape;
            armLogic.OnPrimaryAxisClick -= ConfirmShape;
            armLogic.OnPrimaryAxisTouchUp -= ExitShapeControlling;

            armLogic.OnTriggerDown -= ShootEnergyProjectile;
        }
        private void Update(){
            if(_Debug) UpdateShape(axis);
        }
        [Button]
        private void ShootChargedProjectile(ArmData data) {
            if (_Debug) return;
            if (data.Energy <= 0) return;

            var chargeScale = _EnergyProjectile.transform.localScale + Vector3.one * (data.Energy / data.MaxEnergy);
            GenerateProjectile(true, chargeScale.y);
            data.Energy = 0;

            UpdateScale(data);
        }
        [Button]
        private void ShootEnergyProjectile(ArmData data)
        {
            if (data.Energy <= 0 || ShootCoolDownProcess != null) return;

            data.Energy -= _CostEnergy;
            GenerateProjectile(false);

            UpdateScale(data);
        }
        private void GenerateProjectile(bool overwriteScale, float scale = 1)
        {
            var clone = Instantiate(_EnergyProjectile, transform.root);
            clone.transform.position = _controller.transform.position + (_controller.transform.forward * _SpawnOffset);
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
            var targetScale = Vector3.one * Mathf.Lerp(_ScaleRange.x, _ScaleRange.y, data.Energy / data.MaxEnergy) ;
            if (data.Energy == 0) targetScale = Vector3.zero;
            currentEnergyBall.transform.DOScale(targetScale, 1/_ScalingSpeed);
        }
        private void ConfirmShape(ArmData data) {
            isShapeConfirmed = true;
        }
        private void ExitShapeControlling(ArmData data) {

            isShapeConfirmed = false;
            //StartCoroutine(Delayer(.5f));
        }
        private IEnumerator Delayer(float duration) {
            yield return new WaitForSeconds(duration);
            isShapeConfirmed = false;
        }
        private void UpdateShape(Vector2 axis) {
            if (isShapeConfirmed) return;

            if (axis.x > 0 && axis.y > 0) {
                // Hard and long
                // Sword
                ActivateWeapon(_Sword);

                swordBehavior = GetComponent<SwordBehavior>();
                swordBehavior.Activate(armLogic, armLogic.data, _Sword.GetComponent<LightBeamRigController>());
            }
            if (axis.x > 0 && axis.y <= 0) {
                // Hard and Short
                // Shield
                ActivateWeapon(_Shield);
            }
            if (axis.x < 0 && axis.y < 0) {
                // Soft and short
                // Ball
                currentEnergyBall.GetComponent<MeshRenderer>().enabled = true;
                currentShape.SetActive(false);
            }
            if (axis.x <= 0 && axis.y > 0) {
                // Soft and Long
                // Whip
                ActivateWeapon(_Whip);
            }
        }
        private void ActivateWeapon(GameObject weapon) {
            currentEnergyBall.GetComponent<MeshRenderer>().enabled = false;
            currentShape.SetActive(false);
            weapon.SetActive(true);
            weapon.transform.position = currentEnergyBall.transform.position;
            weapon.transform.forward = _controller.transform.forward;
            weapon.transform.SetParent(currentEnergyBall.transform.parent);
            currentShape = weapon;
        }
        private IEnumerator CoolDown (float duration) {
            yield return new WaitForSeconds(duration);
            StopCoroutine(ShootCoolDownProcess);
            ShootCoolDownProcess = null;
        }
    }
}
