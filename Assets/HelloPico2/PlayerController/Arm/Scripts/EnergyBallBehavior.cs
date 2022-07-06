using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InputDevice.Scripts;
using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace HelloPico2.PlayerController.Arm
{
    [RequireComponent(typeof(ArmLogic))]
    public class EnergyBallBehavior : WeaponBehavior
    {        
        [FoldoutGroup("Charging Energyball Position")][SerializeField] private Transform _Pivot;
        [FoldoutGroup("Charging Energyball Position")][SerializeField] private GameObject _ChargingEnergyBall;
        [FoldoutGroup("Charging Energyball Position")][SerializeField] private Vector2 _ScaleRange;
        [FoldoutGroup("Charging Energyball Position")][SerializeField] private float _ScalingSpeed;

        [FoldoutGroup("Projectile")][SerializeField] private GameObject _EnergyProjectile;
        [FoldoutGroup("Projectile")][SerializeField] private GameObject _ChargedEnergyProjectile;
        [FoldoutGroup("Projectile")][SerializeField] private float _SpawnOffset;
        [FoldoutGroup("Projectile")][SerializeField] private float _ShootSpeed;
        [FoldoutGroup("Projectile")][SerializeField] private float _ChargeShootSpeed;
        [FoldoutGroup("Projectile")][SerializeField] private float _ShootCoolDown;
        [FoldoutGroup("Projectile")][SerializeField] private float _CostEnergy;
        [FoldoutGroup("Projectile")][Tooltip("Projectile Initial Speed Buffer")][Min(0.05f)][SerializeField] private float _SpeedBufferDuration;
        [FoldoutGroup("Projectile")][SerializeField] private AnimationCurve _SpeedBufferEasingCurve;
                
        [FoldoutGroup("Weapon Object")][SerializeField] private GameObject _Sword;
        [FoldoutGroup("Weapon Object")][SerializeField] private GameObject _Whip;
        [FoldoutGroup("Weapon Object")][SerializeField] private GameObject _Shield;
        private EnergyBallBehavior energyBehavior;
        private SwordBehavior swordBehavior;
        private ShieldBehavior shieldBehavior;
                
        [FoldoutGroup("Transition")][SerializeField] private float _TransitionDuration = .5f;
        [FoldoutGroup("Transition")][SerializeField] private Vector3 _SwordFromScale;
        [FoldoutGroup("Transition")][SerializeField] private Ease _TrasitionEaseCurve;

        [FoldoutGroup("Debug")] public bool _OnlyShootProjectileOnEnergyBallState = true;
        [FoldoutGroup("Debug")] public bool _Debug;
        [FoldoutGroup("Debug")] public Vector2 axis;
        [FoldoutGroup("Debug")] public Transform target;

        [FoldoutGroup("Transition")][ReadOnly][SerializeField] private bool hasTransformProcess;
        public Coroutine ShootCoolDownProcess { get; set; }

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
        private WeaponBehavior currentWeaponBehavior;
        private bool isShapeConfirmed = false;

        private void Start()
        {
            Project.EventBus.Subscribe<GainEnergyEventData>(ChargeEnergy);
            Project.EventBus.Subscribe<GainEnergyEventData>(ChargeEnergyBall);
        }
        private void OnEnable()
        {
            armLogic.OnEnergyChanged += UpdateScale;
            armLogic.OnEnergyChanged += CheckEnableGrip;

            armLogic.OnGripUp += ShootChargedProjectile;

            armLogic.OnPrimaryAxisInput += UpdateShape;
            armLogic.OnPrimaryAxisClick += ConfirmShape;
            armLogic.OnPrimaryAxisTouchUp += ExitShapeControlling;

            armLogic.OnTriggerDown += ShootEnergyProjectile;

            currentShape = currentEnergyBall;

            GenerateChargingEnergyBall();
        }
        private void OnDisable()
        {
            armLogic.OnEnergyChanged -= UpdateScale;
            armLogic.OnEnergyChanged -= CheckEnableGrip;

            armLogic.OnGripUp -= ShootChargedProjectile;

            armLogic.OnPrimaryAxisInput -= UpdateShape;
            armLogic.OnPrimaryAxisClick -= ConfirmShape;
            armLogic.OnPrimaryAxisTouchUp -= ExitShapeControlling;

            armLogic.OnTriggerDown -= ShootEnergyProjectile;
        }
        private void CheckEnableGrip(ArmData data) {
            armLogic.data.Controller.selectUsage = (data.Energy < data.MaxEnergy)? InputHelpers.Button.Grip : InputHelpers.Button.None;
        }
        private void ChargeEnergy(GainEnergyEventData eventData)
        {
            if (eventData.InputReceiver.Selector.HandType != armLogic.data.HandType) return;

            armLogic.controllerInteractor.CancelSelect(eventData.Interactable);

            var targetPos = currentEnergyBall.transform.position;

            eventData.Interactable.transform.DOMove(targetPos, armLogic.data.GrabEasingDuration).OnComplete(() =>
            {
                armLogic.data.Energy += eventData.Energy;
                               
                armLogic.data.Energy = Mathf.Clamp(armLogic.data.Energy, 0, armLogic.data.MaxEnergy);

                armLogic.data.WhenGainEnergy?.Invoke();

                Destroy(eventData.Interactable.transform.gameObject); 
                
                armLogic.OnEnergyChanged?.Invoke(armLogic.data);
            });
        }
        [Button]
        private void ShootChargedProjectile(ArmData data)
        {
            if (_Debug) return;
            if (data.Energy <= 0) return;
            if (hasTransformProcess) return;

            var chargeScale = _EnergyProjectile.transform.localScale + Vector3.one * (data.Energy / data.MaxEnergy);
            GenerateProjectile(true, _ChargedEnergyProjectile, _ChargeShootSpeed, chargeScale.y);
            data.Energy = 0;
            armLogic.OnEnergyChanged?.Invoke(data);

            UpdateScale(data);

            data.WhenShootChargedProjectile?.Invoke();
        }
        [Button]
        private void ShootEnergyProjectile(ArmData data)
        {
            if (data.Energy <= 0 || ShootCoolDownProcess != null) return;
            if (_OnlyShootProjectileOnEnergyBallState && currentShape != currentEnergyBall) return;
            if (hasTransformProcess) return;

            data.Energy -= _CostEnergy;
            armLogic.OnEnergyChanged?.Invoke(data);
            GenerateProjectile(false, _EnergyProjectile, _ShootSpeed);

            UpdateScale(data);

            data.WhenShootProjectile?.Invoke();
        }
        private void GenerateProjectile(bool overwriteScale, GameObject prefab, float speed, float scale = 1)
        {
            var clone = Instantiate(prefab, transform.root);
            clone.transform.position = currentEnergyBall.transform.position + (currentEnergyBall.transform.forward * _SpawnOffset);
            clone.transform.forward = armLogic.data.Controller.transform.forward;
            if (overwriteScale) clone.transform.localScale *= scale;

            clone.GetComponent<ProjectileController>().ProjectileSetUp(speed, _SpeedBufferDuration, _SpeedBufferEasingCurve, target);
                        
            ShootCoolDownProcess = StartCoroutine(CoolDown(_ShootCoolDown));
        }
        private void ChargeEnergyBall(GainEnergyEventData eventData) {
            // Check same arm
            if (armLogic.data.HandType != eventData.InputReceiver.Selector.HandType) return;

            GenerateChargingEnergyBall();

            if (!armLogic.CheckHasEnergy() && !currentEnergyBall.activeSelf)
                currentEnergyBall.SetActive(true);
        }
        private void GenerateChargingEnergyBall() {
            if (currentEnergyBall == null)
            {
                currentEnergyBall = Instantiate(_ChargingEnergyBall, _Pivot);
                currentEnergyBall.transform.localPosition = _DefaultOffset +
                new Vector3(0, 0, _OffsetRange.x);

                if (!armLogic.CheckHasEnergy())
                    currentEnergyBall.SetActive(false);
            }
        }
        private void UpdateScale(ArmData data) {
            var targetScale = Vector3.one * Mathf.Lerp(_ScaleRange.x, _ScaleRange.y, data.Energy / data.MaxEnergy) ;
            if (data.Energy == 0) targetScale = Vector3.zero;
            currentEnergyBall.transform.DOScale(targetScale, 1/_ScalingSpeed);
            var ratio = (targetScale.x - _ScaleRange.x) / (_ScaleRange.y - _ScaleRange.x);
            currentEnergyBall.transform.DOLocalMove(
                _DefaultOffset +
                new Vector3(0, 0,
                _OffsetRange.x + ((_OffsetRange.y - _OffsetRange.x) * ratio)),
                1/_ScalingSpeed);
        }
        private void ConfirmShape(ArmData data) {
            isShapeConfirmed = true;
        }
        private void ExitShapeControlling(ArmData data) {
            isShapeConfirmed = false;
        }
        private void UpdateShape(Vector2 axis) {
            //if (isShapeConfirmed) return;
            
            if (hasTransformProcess) return;

            // Force activate Energy ball when player has no energy
            if (Mathf.Abs(axis.y) <= 0.1f || !armLogic.CheckHasEnergy())
            {
                if (energyBehavior == null)
                    energyBehavior = GetComponent<EnergyBallBehavior>();

                if (currentShape == currentEnergyBall) return;

                var fromScale = (currentShape)? currentShape.transform.localScale: Vector3.zero;
                if(!armLogic.CheckHasEnergy()) currentEnergyBall.transform.localScale = Vector3.zero;

                // Ball
                ActivateWeapon(currentEnergyBall);

                energyBehavior.Activate(armLogic, armLogic.data, currentEnergyBall, fromScale);
                currentWeaponBehavior = energyBehavior;
            }

            if (!armLogic.CheckHasEnergy()) return;

            if (axis.y > 0.1f) {
                if (swordBehavior == null)
                    swordBehavior = GetComponent<SwordBehavior>();

                if (currentShape == _Sword) return;

                var fromScale = (currentShape) ? currentShape.transform.localScale : Vector3.zero;

                ActivateWeapon(_Sword);

                swordBehavior.Activate(armLogic, armLogic.data, _Sword, fromScale);
                currentWeaponBehavior = swordBehavior;
            }
            if (axis.y < -0.1f) {
                if (shieldBehavior == null)
                    shieldBehavior = GetComponent<ShieldBehavior>();

                if (currentShape == _Shield) return;

                var fromScale = (currentShape) ? currentShape.transform.localScale : Vector3.zero;

                ActivateWeapon(_Shield);                

                shieldBehavior.Activate(armLogic, armLogic.data, _Shield, fromScale);
                currentWeaponBehavior = shieldBehavior;
            }
            
        }
        private void ActivateWeapon(GameObject weapon) {            
            if (currentShape)
            {
                hasTransformProcess = true;

                if (currentWeaponBehavior)
                {
                    // Setting up the events that will run after deactivate the current weapon
                    currentWeaponBehavior._FinishedDeactivate = delegate ()
                    {
                        weapon.SetActive(true);

                        hasTransformProcess = false;
                    };

                    currentWeaponBehavior.Deactivate(currentShape); 
                }
                else
                {
                    // For those behavior not yet fully implemented
                    currentShape?.SetActive(false);
                    weapon.SetActive(true);

                    hasTransformProcess = false;
                }
            }
            
            // Positioning the weapon
            weapon.transform.position = currentEnergyBall.transform.position;
            weapon.transform.forward = armLogic.data.Controller.transform.forward;
            weapon.transform.SetParent(currentEnergyBall.transform.parent);
            currentShape = weapon;
        }
        private IEnumerator CoolDown (float duration) {
            yield return new WaitForSeconds(duration);
            
            if(ShootCoolDownProcess != null)
                StopCoroutine(ShootCoolDownProcess);

            ShootCoolDownProcess = null;
        }
        #region WeaponBehavior
        public override void Activate(ArmLogic Logic, ArmData data, GameObject Obj, Vector3 fromScale)
        {
            base.Activate(Logic, data, Obj, fromScale);

            if (currentWeaponBehavior == null) return;

            currentWeaponBehavior._FinishedDeactivate = delegate ()
            {
                Obj.SetActive(true);
                //  TODO: Scaling control
                Obj.transform.DOScale(Obj.transform.localScale, _TransitionDuration).From(_SwordFromScale).SetEase(_TrasitionEaseCurve);
                //Obj.transform.DOScale(Obj.transform.localScale, _TransitionDuration).From(Vector3.one * fromScale.x).SetEase(_TrasitionEaseCurve);
                //print("Energyball Deactivate");

                hasTransformProcess = false;
            };
        }
        public override void Deactivate(GameObject obj)
        {
            base.Deactivate(obj);
            var originalScale = obj.transform.localScale;
            obj.transform.DOScale(_SwordFromScale, _TransitionDuration).SetEase(_TrasitionEaseCurve).OnComplete(() => {
                obj.SetActive(false);   
                obj.transform.localScale = originalScale;   
                _FinishedDeactivate?.Invoke();
            });
        }        
        #endregion        
    }
}
