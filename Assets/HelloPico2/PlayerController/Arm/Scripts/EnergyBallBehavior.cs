using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InputDevice.Scripts;
using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;
using Game.Project;
using DG.Tweening;
using HelloPico2.LevelTool;

namespace HelloPico2.PlayerController.Arm
{
    [RequireComponent(typeof(ArmLogic))]
    public class EnergyBallBehavior : WeaponBehavior
    {
        #region Variables
        public enum GrabReleaseType { OnGripUp, OnGripTouchRelease}
        [SerializeField] private GrabReleaseType _GrabReleaseType = GrabReleaseType.OnGripUp;
        [FoldoutGroup("Charging Energyball Position")][SerializeField] private Transform _Pivot;
        [FoldoutGroup("Charging Energyball Position")][SerializeField] private GameObject _ChargingEnergyBall;
        [FoldoutGroup("Charging Energyball Position")][SerializeField] private Vector2 _ScaleRange;
        [FoldoutGroup("Charging Energyball Position")][SerializeField] private float _ScalingSpeed;

        public enum LockOnType { SphereCast, Cone}
        [FoldoutGroup("Lock On Target")][SerializeField] private LockOnType _LockOnType;
        [FoldoutGroup("Lock On Target")][Min(0)][SerializeField] private float _CheckSphererRadius;
        [FoldoutGroup("Lock On Target")][ShowIf("_LockOnType", LockOnType.Cone)][Min(0)][SerializeField] private float _CheckEndSphererRadius;
        [FoldoutGroup("Lock On Target")][ShowIf("_LockOnType", LockOnType.Cone)][Range(10,30)][SerializeField] private float _AnglePercision = 10;
        [FoldoutGroup("Lock On Target")][ShowIf("_LockOnType", LockOnType.Cone)][Range(1,10)][SerializeField] private float _RaycastPercision = 10;
        [FoldoutGroup("Lock On Target")][SerializeField] private float _Distance;
        [FoldoutGroup("Lock On Target")][SerializeField] private LayerMask _LayerMask;

        [FoldoutGroup("Projectile")][SerializeField] private GameObject _EnergyProjectile;
        [FoldoutGroup("Projectile")][SerializeField] private GameObject _ChargedEnergyProjectile;
        [FoldoutGroup("Projectile")][SerializeField] private bool _Homing;
        [FoldoutGroup("Projectile")][SerializeField] private Transform _TestTarget;
        [FoldoutGroup("Projectile")][SerializeField] private float _SpawnOffset;
        [FoldoutGroup("Projectile")][SerializeField] private float _ShootSpeed;
        [FoldoutGroup("Projectile")][SerializeField] private float _ChargeShootSpeed;
        [FoldoutGroup("Projectile")][SerializeField] private float _ChargedShootCoolDown;
        [FoldoutGroup("Projectile")][SerializeField] private float _ShootCoolDown;
        [FoldoutGroup("Projectile")][SerializeField] private float _CostEnergy;
        [FoldoutGroup("Projectile")][SerializeField] private float _FullEnergyBallCostEnergy;
        [FoldoutGroup("Projectile")][SerializeField] private float _ShootingActionCDDurationAfterFullEnergyBall = 1;
        [FoldoutGroup("Projectile")][Tooltip("Projectile Initial Speed Buffer")][Min(0.05f)][SerializeField] private float _SpeedBufferDuration;
        [FoldoutGroup("Projectile")][SerializeField] private AnimationCurve _SpeedBufferEasingCurve;
                
        [FoldoutGroup("Weapon Object")][SerializeField] private GameObject _Sword;
        [FoldoutGroup("Weapon Object")][SerializeField] private GameObject _Whip;
        [FoldoutGroup("Weapon Object")][SerializeField] private GameObject _Shield;
        
                
        [FoldoutGroup("Transition")][SerializeField] private float _TransitionDuration = .5f;
        [FoldoutGroup("Transition")][SerializeField] private Vector3 _SwordFromScale;
        [FoldoutGroup("Transition")][SerializeField] private Ease _TrasitionEaseCurve;

        [FoldoutGroup("Debug")] public bool _OnlyShootProjectileOnEnergyBallState = true;
        
        [FoldoutGroup("Transition")][ReadOnly][SerializeField] private bool _HasTransformProcess;
        private SwordBehavior _SwordBehavior;
        private ShieldBehavior _ShieldBehavior;
        private EnergyBallBehavior energyBehavior;
        private bool _Charged;

        public SwordBehavior swordBehavior { get { 
                if(_SwordBehavior == null)
                    _SwordBehavior = GetComponent<SwordBehavior>();

                return _SwordBehavior;
            } 
        }
        public ShieldBehavior shieldBehavior
        {
            get
            {
                if (_ShieldBehavior == null)
                    _ShieldBehavior = GetComponent<ShieldBehavior>();

                return _ShieldBehavior;
            }
        }
        public LockOnType ChangeLockOnType { get { return _LockOnType; } set { _LockOnType = value; } }
        public float CheckSphererRadius { get { return _CheckSphererRadius; } set { _CheckSphererRadius = value; } }
        public float CheckEndSphererRadius { get { return _CheckEndSphererRadius; } set { _CheckEndSphererRadius = value; } }
        public float AnglePercision { get { return _AnglePercision; } set { _AnglePercision = value; } }
        public float RaycastPercision { get { return _RaycastPercision; }set { _RaycastPercision = value; } }
        public float Distance { get { return _Distance; } set { _Distance = value; } }
        public LayerMask LayerMask { get { return _LayerMask; } set { _LayerMask = value; } }

        public GameObject ChargedEnergyProjectile { get { return _ChargedEnergyProjectile; } set { _ChargedEnergyProjectile = value; } }
        public bool hasTransformProcess { get { return _HasTransformProcess; } }
        public Coroutine ShootCoolDownProcess { get; set; }
        public Coroutine BombShootCoolDownProcess { get; set; }
        public bool OverWriteProjectileLifeTime { get; set; }
        public float ModifiedProjectileLifeTime { get; set; }

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
        Game.Project.ColdDownTimer EnergyBallEmptySoundCD;
        Game.Project.ColdDownTimer BombEmptySoundCD;
        public List<IGainEnergyFeedback> GainEnergyFeedback = new List<IGainEnergyFeedback>();
        public List<IShootingFeedback> ShootingFeedback = new List<IShootingFeedback>();
        public List<IFullEnergyFeedback> FullEnergyFeedback = new List<IFullEnergyFeedback>();

        private DeviceInputDetected currentDeviceInputDetected { get; set; }
        private ColdDownTimer shootingCDAfterFullChargedShoot;
        #endregion
        #region Public Method    
        public bool isCurrentWeaponEnergyBall() {
            return currentWeaponBehavior == energyBehavior;
        }
        public void ChargeEnergyDirectly(float energy)
        {
            // Charge Energy
            armLogic.data.Energy = energy;

            armLogic.data.WhenGainEnergy?.Invoke();

            armLogic.OnEnergyChanged?.Invoke(armLogic.data);

            // Charge Energy ball
            GenerateChargingEnergyBall();

            // Feedbacks
            GainEnergyFeedback.ForEach(x => x.OnNotify(armLogic.data, this));

            if (energy != 0)
                currentEnergyBall.SetActive(true);
            else            
                currentEnergyBall.transform.localScale = Vector3.zero;             
        }
        public void SetShootCoolDown(float duration) {
            _ShootCoolDown = duration;
        }
        public void SetShootSpeed(float speed) { 
            _ShootSpeed = speed;
        }        
        #endregion

        private void Start()
        {
            Project.EventBus.Subscribe<GainEnergyEventData>(ChargeEnergy);
            Project.EventBus.Subscribe<GainEnergyEventData>(ChargeEnergyBall);

            Project.EventBus.Subscribe<GainBombEventData>(ChargeBomb);

            EnergyBallEmptySoundCD = new Game.Project.ColdDownTimer(armLogic.data.ShootEmptyEnergyCoolDownDuration);
            BombEmptySoundCD = new Game.Project.ColdDownTimer(armLogic.data.ShootEmptyBombCoolDownDuration);

            shootingCDAfterFullChargedShoot = new ColdDownTimer(_ShootingActionCDDurationAfterFullEnergyBall);
        }
        private void Update()
        {
            //if(CheckTarget())
            //    CheckTarget().gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            armLogic.OnEnergyChanged += UpdateScale;
            armLogic.OnEnergyChanged += CheckChargingFeedbacks;
            //armLogic.OnEnergyChanged += CheckEnableGrip;

            //if(_GrabReleaseType == GrabReleaseType.OnGripUp)            
            //    armLogic.OnGripUp += ShootChargedProjectile;
            //if(_GrabReleaseType == GrabReleaseType.OnGripTouchRelease)            
            //    armLogic.OnGripTouch += ShootChargedProjectile;

            armLogic.OnPrimaryButtonClick += ShootChargedProjectile;
            armLogic.OnSecondaryButtonClick += ShootChargedProjectile;

            armLogic.OnPrimaryButtonClickOnce += InvalidBombShoot;
            armLogic.OnSecondaryButtonClickOnce += InvalidBombShoot;

            armLogic.OnPrimaryAxisInput += UpdateShape;
            armLogic.OnPrimaryAxisClick += ConfirmShape;
            armLogic.OnPrimaryAxisTouchUp += ExitShapeControlling;

            armLogic.OnUpdateInput += GetCurrentDeviceInput;
            armLogic.OnTriggerDown += ShootEnergyProjectile;

            armLogic.OnTriggerDownOnce += InvalidShoot;

            currentShape = currentEnergyBall;

            GenerateChargingEnergyBall();
        }
        private void OnDisable()
        {
            armLogic.OnEnergyChanged -= UpdateScale;
            armLogic.OnEnergyChanged -= CheckChargingFeedbacks;
            //armLogic.OnEnergyChanged -= CheckEnableGrip;

            //if (_GrabReleaseType == GrabReleaseType.OnGripUp)
            //    armLogic.OnGripUp -= ShootChargedProjectile;
            //if (_GrabReleaseType == GrabReleaseType.OnGripTouchRelease)
            //    armLogic.OnGripTouch -= ShootChargedProjectile;
            
            armLogic.OnPrimaryButtonClick -= ShootChargedProjectile;
            armLogic.OnSecondaryButtonClick -= ShootChargedProjectile;

            armLogic.OnPrimaryButtonClickOnce -= InvalidBombShoot;
            armLogic.OnSecondaryButtonClickOnce -= InvalidBombShoot;

            armLogic.OnPrimaryAxisInput -= UpdateShape;
            armLogic.OnPrimaryAxisClick -= ConfirmShape;
            armLogic.OnPrimaryAxisTouchUp -= ExitShapeControlling;

            armLogic.OnUpdateInput -= GetCurrentDeviceInput;
            armLogic.OnTriggerDown -= ShootEnergyProjectile;

            armLogic.OnTriggerDownOnce -= InvalidShoot;
        }
        private void CheckEnableGrip(ArmData data) {
            armLogic.data.Controller.selectUsage = (data.Energy < data.MaxEnergy)? InputHelpers.Button.Grip : InputHelpers.Button.None;
        }
        private void ChargeEnergy(GainEnergyEventData eventData)
        {
            if (eventData.InputReceiver.Selector.HandType != armLogic.data.HandType) return;

            armLogic.controllerInteractor.CancelSelect(eventData.Interactable);

            var targetPos = currentEnergyBall.transform.position;

            eventData.Interactable.transform.SetParent(eventData.Interactable.transform.root);

            eventData.Interactable.transform.DOMove(targetPos, armLogic.data.GrabEasingDuration).SetEase(armLogic.data.GrabEasingCurve).OnComplete(() =>
            {
                armLogic.data.Energy += eventData.Energy;

                armLogic.data.WhenGainEnergy?.Invoke();

                armLogic.OnEnergyChanged?.Invoke(armLogic.data);

                //AudioPlayerHelper.PlayAudio(armLogic.data.GainEnergyClipName, transform.position);

                // Feedbacks
                GainEnergyFeedback.ForEach(x => x.OnNotify(armLogic.data, this));

                Destroy(eventData.Interactable.transform.gameObject);
            });
        }
        private void CheckChargingFeedbacks(ArmData data)
        {
            if (armLogic.CheckFullEnergy())
            {
                FullEnergyFeedback.ForEach(x => x.OnFullEnergyNotify(armLogic.data.HandType));

                if (!_Charged)
                {
                    _Charged = true;
                    AudioPlayerHelper.PlayAudio(armLogic.data.EnergyballFullyCharged, transform.position);
                }
            }
            else
            {
                _Charged = false;
                FullEnergyFeedback.ForEach(x => x.ExitFullEnergyNotify(armLogic.data.HandType));
            }
        }
             
        private void ChargeBomb(GainBombEventData eventData)
        {
            if (eventData.InputReceiver.Selector.HandType != armLogic.data.HandType) return;

            armLogic.controllerInteractor.CancelSelect(eventData.Interactable);

            var targetPos = currentEnergyBall.transform.position;

            eventData.Interactable.transform.SetParent(eventData.Interactable.transform.root);

            eventData.Interactable.transform.DOMove(targetPos, armLogic.data.GrabEasingDuration).SetEase(armLogic.data.GrabEasingCurve).OnComplete(() =>
            {
                armLogic.data.bombAmount += eventData.Amount;

                armLogic.data.WhenGainBomb?.Invoke();

                AudioPlayerHelper.PlayAudio(armLogic.data.GainBombClipName, transform.position);

                Destroy(eventData.Interactable.transform.gameObject);
            });
        }
        private Transform CheckTarget() {
            Ray ray = new Ray();   
            ray.origin = currentEnergyBall.transform.position + (currentEnergyBall.transform.forward * _SpawnOffset);
            ray.direction = armLogic.data.Controller.transform.forward;
            if (_LockOnType == LockOnType.SphereCast)
            {
                RaycastHit[] hitInfos = new RaycastHit[3];

                if (Physics.SphereCastNonAlloc(ray, _CheckSphererRadius, hitInfos, _Distance, _LayerMask) > 0)                
                    return hitInfos[0].transform;                
            }
            if (_LockOnType == LockOnType.Cone)
            {
                var angle = _AnglePercision;
                var percision = _RaycastPercision;
                var loops = 360 / angle;
                var radiusDecrement = _CheckSphererRadius / percision;
                var endRradiusDecrement = _CheckEndSphererRadius / percision;
                RaycastHit hitInfo;

                for (int i = 0; i < percision; i++)
                {
                    var currentRadius = _CheckSphererRadius - radiusDecrement * i;
                    var currentEndRadius = _CheckEndSphererRadius - endRradiusDecrement * i;                    

                    for (int j = 0; j < loops; j++)
                    {
                        var angleDir = Quaternion.AngleAxis(angle * j, armLogic.data.Controller.transform.forward) * armLogic.data.Controller.transform.right;
                        var Pos = ray.origin + angleDir * currentRadius;
                        var target = ray.origin + armLogic.data.Controller.transform.forward * _Distance + angleDir * currentEndRadius;
                        var dir = (target - Pos).normalized;
                        
                        if(Physics.Raycast(Pos, dir, out hitInfo, _Distance, _LayerMask))
                            return hitInfo.transform;
                    }
                }
            }
            return null;
        }
        private void GetCurrentDeviceInput(DeviceInputDetected obj) {
            currentDeviceInputDetected = obj;
        }
        private void ShootChargedProjectile(ArmData data)
        {
            if (data.bombAmount <= 0) {
                if (BombEmptySoundCD.CanInvoke())
                {
                    AudioPlayerHelper.PlayAudio(data.ShootWhenNoBombClipName, transform.position);
                    BombEmptySoundCD.Reset();
                    data.WhenNoBombShoot?.Invoke();
                }
                return; 
            }
            if (BombShootCoolDownProcess != null) return;
            if (_HasTransformProcess) return;

            GenerateProjectile(true, _ChargedEnergyProjectile, _ChargeShootSpeed);
            // CD
            BombShootCoolDownProcess = StartCoroutine(BombCoolDown(_ChargedShootCoolDown));

            data.bombAmount--;

            data.WhenShootChargedProjectile?.Invoke();
        }
        private void ShootFulEnergyProjectile(ArmData data)
        {
            data.Energy -= _FullEnergyBallCostEnergy;

            armLogic.OnEnergyChanged?.Invoke(data); 
            
            GenerateProjectile(true, _ChargedEnergyProjectile, _ChargeShootSpeed);

            // Feedbacks
            ShootingFeedback.ForEach(x => x.OnNotify(data.HandType));

            // CD
            ShootCoolDownProcess = StartCoroutine(CoolDown(_ShootCoolDown));
            shootingCDAfterFullChargedShoot.Reset();

            UpdateScale(data);

            data.WhenShootProjectile?.Invoke();
        }
        private void ShootEnergyProjectile(ArmData data)
        {
            if (!shootingCDAfterFullChargedShoot.CanInvoke()) return;
            if (data.Energy <= 0)
            {
                if (EnergyBallEmptySoundCD.CanInvoke())
                {
                    AudioPlayerHelper.PlayAudio(data.ShootWhenNoEnergyClipName, transform.position);
                    data.WhenNoEnergyShoot?.Invoke();
                    EnergyBallEmptySoundCD.Reset();
                }
                return;
            }
            if (ShootCoolDownProcess != null) return;
            if (_OnlyShootProjectileOnEnergyBallState && currentShape != currentEnergyBall) return;
            if (_HasTransformProcess) return;
            if (armLogic.CheckFullEnergy()) { ShootFulEnergyProjectile(data); return; }

            data.Energy -= _CostEnergy;
            armLogic.OnEnergyChanged?.Invoke(data);
            GenerateProjectile(false, _EnergyProjectile, _ShootSpeed, 1, _Homing);

            // Feedbacks
            ShootingFeedback.ForEach(x => x.OnNotify(data.HandType));

            // CD
            ShootCoolDownProcess = StartCoroutine(CoolDown(_ShootCoolDown));

            UpdateScale(data);

            data.WhenShootProjectile?.Invoke();
        }
        private void InvalidShoot(ArmData data) {
            if (data.Energy <= 0)
            {
                AudioPlayerHelper.PlayAudio(data.ShootWhenNoEnergyClipName, transform.position);
                data.WhenNoEnergyShoot?.Invoke();
                EnergyBallEmptySoundCD.Reset();
            }
        }
        private void InvalidBombShoot(ArmData data)
        {
            if (data.bombAmount <= 0)
            {
                AudioPlayerHelper.PlayAudio(data.ShootWhenNoBombClipName, transform.position);
                data.WhenNoBombShoot?.Invoke();
                BombEmptySoundCD.Reset();
            }
        }
        private void GenerateProjectile(bool overwriteScale, GameObject prefab, float speed, float scale = 1, bool homing = false)
        {
            var clone = Instantiate(prefab, transform.root);
            clone.transform.position = currentEnergyBall.transform.position + (currentEnergyBall.transform.forward * _SpawnOffset);
            clone.transform.forward = armLogic.data.Controller.transform.forward;
            if (overwriteScale) clone.transform.localScale *= scale;
            
            if (homing) {
                _TestTarget = null;
                var target = CheckTarget();
                if (target != null) {
                    _TestTarget = target;
                }
            }
            if(!OverWriteProjectileLifeTime)            
                clone.GetComponent<ProjectileController>().ProjectileSetUp(speed, _SpeedBufferDuration, _SpeedBufferEasingCurve, currentDeviceInputDetected, _TestTarget, homing);
            else
                clone.GetComponent<ProjectileController>().ProjectileSetUp(speed, _SpeedBufferDuration, _SpeedBufferEasingCurve, currentDeviceInputDetected, _TestTarget, homing, ModifiedProjectileLifeTime);

            SendAimingPercisionData(currentEnergyBall.transform, _TestTarget);
        }
        private void ChargeEnergyBall(GainEnergyEventData eventData) {
            // Check same arm
            if (armLogic.data.HandType != eventData.InputReceiver.Selector.HandType) return;

            GenerateChargingEnergyBall();

            if (!armLogic.CheckHasEnergy() && !currentEnergyBall.activeSelf)
                currentEnergyBall.SetActive(true);
        }
        private void SendAimingPercisionData(Transform controller, Transform target) {
            Rating_System.RatingInputRequested rating = new Rating_System.RatingInputRequested(controller, target);
            Project.EventBus.Post(rating);
        }
        private void GenerateChargingEnergyBall() {
            if (currentEnergyBall == null)
            {
                currentEnergyBall = Instantiate(_ChargingEnergyBall, _Pivot);
                currentEnergyBall.transform.localPosition = _DefaultOffset +
                new Vector3(0, 0, _OffsetRange.x);

                if (!armLogic.CheckHasEnergy())
                    currentEnergyBall.SetActive(false);

                if (currentEnergyBall.TryGetComponent<IGainEnergyFeedback>(out var gainEnergyFeedback)){
                    GainEnergyFeedback.Add(gainEnergyFeedback);
                }
                if (currentEnergyBall.TryGetComponent<IShootingFeedback>(out var shootingFeedback)) { 
                    ShootingFeedback.Add(shootingFeedback);                
                }
                if (currentEnergyBall.TryGetComponent<IFullEnergyFeedback>(out var fullEnergyFeedback)) {
                    FullEnergyFeedback.Add(fullEnergyFeedback);                
                }
            }
        }
        public void ChangeChargingEnergyBall(GameObject chargingEnergyball)
        {
            _ChargingEnergyBall = chargingEnergyball;

            if (currentEnergyBall == null)
            {
                GenerateChargingEnergyBall();
            }
            else {
                Destroy(currentEnergyBall);
                currentEnergyBall = Instantiate(_ChargingEnergyBall, _Pivot);

                currentShape = currentEnergyBall;

                currentEnergyBall.transform.localPosition = _DefaultOffset +
                new Vector3(0, 0, _OffsetRange.x);

                if (!armLogic.CheckHasEnergy())
                    currentEnergyBall.SetActive(false);

                if (currentEnergyBall.TryGetComponent<IGainEnergyFeedback>(out var gainEnergyFeedback))
                {
                    GainEnergyFeedback.Clear();
                    GainEnergyFeedback.Add(gainEnergyFeedback);
                }
                if (currentEnergyBall.TryGetComponent<IShootingFeedback>(out var shootingFeedback))
                {
                    ShootingFeedback.Clear();
                    ShootingFeedback.Add(shootingFeedback);
                }
                if (currentEnergyBall.TryGetComponent<IFullEnergyFeedback>(out var fullEnergyFeedback))
                {
                    FullEnergyFeedback.Clear();
                    FullEnergyFeedback.Add(fullEnergyFeedback);
                }
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
            
            if (_HasTransformProcess) return;

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
                if (currentShape == _Sword) return;

                var fromScale = (currentShape) ? currentShape.transform.localScale : Vector3.zero;

                ActivateWeapon(_Sword);

                swordBehavior.Activate(armLogic, armLogic.data, _Sword, fromScale);
                currentWeaponBehavior = swordBehavior;
            }
            if (axis.y < -0.1f) {
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
                _HasTransformProcess = true;

                if (currentWeaponBehavior)
                {
                    // Setting up the events that will run after deactivate the current weapon
                    currentWeaponBehavior._FinishedDeactivate = delegate ()
                    {
                        weapon.SetActive(true);

                        _HasTransformProcess = false;
                    };

                    currentWeaponBehavior.Deactivate(currentShape);
                    if (currentShape.TryGetComponent<IActivationNotify>(out var notify))
                        notify.OnNotifyDeactivate();
                }
                else
                {
                    // For those behavior not yet fully implemented
                    currentShape?.SetActive(false);
                    weapon.SetActive(true);

                    _HasTransformProcess = false;
                }
            }
            
            // Positioning the weapon
            weapon.transform.position = currentEnergyBall.transform.position;
            weapon.transform.forward = armLogic.data.Controller.transform.forward;
            weapon.transform.SetParent(currentEnergyBall.transform.parent);
            currentShape = weapon;
            if (weapon.TryGetComponent<IActivationNotify>(out var notifyActivate))
                notifyActivate.OnNotifyActivate();
        }
        private IEnumerator CoolDown (float duration) {
            yield return new WaitForSeconds(duration);

            if (ShootCoolDownProcess != null)
                StopCoroutine(ShootCoolDownProcess);

            ShootCoolDownProcess = null;
        }
        private IEnumerator BombCoolDown(float duration)
        {
            yield return new WaitForSeconds(duration);

            if (BombShootCoolDownProcess != null)
                StopCoroutine(BombShootCoolDownProcess);

            BombShootCoolDownProcess = null;
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

                _HasTransformProcess = false;
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
        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            Color col = new Color(1, 0, 0, 0.3f);
            Gizmos.color = col; 
            Gizmos.DrawWireSphere(transform.position, _CheckSphererRadius);    
            Gizmos.DrawWireSphere(transform.position + transform.forward * _Distance, _CheckSphererRadius);

            GUI.color = col;
            Handles.Label(transform.position + transform.forward * _Distance / 2, "Homing Target Range");
            if (_LockOnType == LockOnType.SphereCast)
            {
                Vector3 startDir, next;
                int rayAmount = Mathf.Clamp(Mathf.FloorToInt(50 * _CheckSphererRadius / 3), 10, 100);
                float angle = 360f / rayAmount;
                for (int i = 0; i < rayAmount; i++)
                {
                    startDir = transform.right;
                    next = transform.position + Quaternion.Euler(0, 0, angle * i) * startDir * _CheckSphererRadius;
                    Gizmos.DrawRay(next, transform.forward * _Distance);
                }
            }
            if (_LockOnType == LockOnType.Cone)
            {
                Ray ray = new Ray();
                ray.origin = armLogic.data.Controller.transform.position;
                ray.direction = armLogic.data.Controller.transform.forward;

                var angle = _AnglePercision;
                var percision = _RaycastPercision;
                var loops = 360 / angle;
                var radiusDecrement = _CheckSphererRadius / percision;
                var endRradiusDecrement = _CheckEndSphererRadius / percision;

                for (int i = 0; i < percision; i++)
                {
                    var currentRadius = _CheckSphererRadius - radiusDecrement * i;
                    var currentEndRadius = _CheckEndSphererRadius - endRradiusDecrement * i;
                    col.a = Mathf.Clamp(col.a - (0.3f / percision) * i, .03f, 1);
                    Gizmos.color = col;

                    for (int j = 0; j < loops; j++)
                    {
                        var angleDir = Quaternion.AngleAxis(angle * j, armLogic.data.Controller.transform.forward) * armLogic.data.Controller.transform.right;
                        var Pos = ray.origin + angleDir * currentRadius;
                        var target = ray.origin + armLogic.data.Controller.transform.forward * _Distance + angleDir * currentEndRadius;
                        var dir = (target - Pos).normalized;
                        Gizmos.DrawLine(Pos, target);
                    }
                }
            }
#endif
        }
        #endregion
    }
}
