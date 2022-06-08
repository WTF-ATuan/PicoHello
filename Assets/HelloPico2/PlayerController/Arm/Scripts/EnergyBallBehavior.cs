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
        [Header("Charging Energyball Position Settings")]
        [SerializeField] private Transform _Pivot;
        [SerializeField] private GameObject _ChargingEnergyBall;
        [SerializeField] private Vector3 _DefaultOffset = new Vector3(0,0,0);
        [SerializeField] private Vector2 _ScaleRange;
        [SerializeField] private Vector2 _OffsetRange;
        [SerializeField] private float _ScalingSpeed;

        [Header("Projectile Settings")]
        [SerializeField] private GameObject _EnergyProjectile;
        [SerializeField] private GameObject _ChargedEnergyProjectile;
        [SerializeField] private float _SpawnOffset;
        [SerializeField] private float _ShootSpeed;
        [SerializeField] private float _ChargeShootSpeed;
        [SerializeField] private float _ShootCoolDown;
        [SerializeField] private float _CostEnergy;
        [SerializeField] private float _SpeedBufferDuration;

        [Header("Shape Settings")]
        [SerializeField] private GameObject _Sword;
        [SerializeField] private GameObject _Whip;
        [SerializeField] private GameObject _Shield;
        private EnergyBallBehavior energyBehavior;
        private SwordBehavior swordBehavior;
        private ShieldBehavior shieldBehavior;
        [Header("Transition Settings")]
        [SerializeField] private float _TransitionDuration = .5f;
        [SerializeField] private Vector3 _SwordFromScale;        
        [SerializeField] private Ease _TrasitionEaseCurve;

        [FoldoutGroup("Debug")] public bool _Debug;
        [FoldoutGroup("Debug")] public Vector2 axis;
        [FoldoutGroup("Debug")] public Transform target;

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
        private void ShootChargedProjectile(ArmData data) {
            if (_Debug) return;
            if (data.Energy <= 0) return;

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

            clone.GetComponent<ProjectileController>().ProjectileSetUp(speed, _SpeedBufferDuration, target);
                        
            ShootCoolDownProcess = StartCoroutine(CoolDown(_ShootCoolDown));
        }
        private void ChargeEnergyBall(GainEnergyEventData eventData) {
            // Check same arm
            if (armLogic.data.HandType != eventData.InputReceiver.Selector.HandType) return;

            GenerateChargingEnergyBall();
        }
        private void GenerateChargingEnergyBall() {
            if (currentEnergyBall == null)
            {
                currentEnergyBall = Instantiate(_ChargingEnergyBall, _Pivot);
                currentEnergyBall.transform.localPosition = _DefaultOffset;
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

            if (Mathf.Abs(axis.y) <= 0.1f)
            {
                if (energyBehavior == null)
                    energyBehavior = GetComponent<EnergyBallBehavior>();

                if (currentShape == currentEnergyBall) return;

                var fromScale = (currentShape)? currentShape.transform.localScale: Vector3.zero;

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
                if (currentWeaponBehavior)
                {
                    // Setting up the events that will run after deactivate the current weapon
                    currentWeaponBehavior._FinishedDeactivate = delegate ()
                    {
                        weapon.SetActive(true);                        
                    };

                    currentWeaponBehavior.Deactivate(currentShape); 
                }
                else
                {
                    // For those behavior not yet fully implemented
                    currentShape?.SetActive(false);
                    weapon.SetActive(true);
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
