using DG.Tweening;
using HelloPico2.InputDevice;
using HelloPico2.InputDevice.Scripts;
using HelloPico2.PlayerController.Arm;
using HelloPico2.PlayerController.Player;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltEvents;
using UnityEngine;

namespace HelloPico2.PlayerController.BeamCharge
{
    public class PlayerTwoHandBehavior : MonoBehaviour
    {
        [System.Serializable]
        public struct PunchData {
            public Vector3 PunchScale;
            public float PunchDuration;
            public int Vibrato;
        }
        [Header("Reference Settings")]
        [ReadOnly][SerializeField] private string _PlayersHandTag = "InteractCollider";
        [ReadOnly][SerializeField] private string _PlayerTag = "Player";
        //[ReadOnly]
        [SerializeField] private GameObject[] _PlayersHands;
        public PickableEnergy[] _PickableEnergys;
        public GameObject[] _PickableEnergyObjects;
        public LineRendererDrawer _LineRendererDrawer;
        public Player_BeamCharge_Controller _BeamChargeController;
        public Transform[] _CarryTheseVFXs;
        [ReadOnly][SerializeField] private ControllerVibrator _ControllerVibratorL, _ControllerVibratorR;

        [Header("Grabbing")]
        [SerializeField] private bool _UseAutoGrab = true;
        [ShowIf("_UseAutoGrab")][SerializeField] private float _AutoGrabDelayDuration = 2;
        [ShowIf("_UseAutoGrab")][SerializeField] private float _AutoGrabDuration = 2;
        [ShowIf("_UseAutoGrab")][SerializeField] private AnimationCurve _AutoGrabEase;

        [Header("Merging")]
        public float _StartCombineDistance = 10;
        public float _StartCombineDuration = 3;
        public Vector2 _PunchEnergyPeriod = new Vector2( 0.3f, 0.1f);
        public AnimationCurve _PeriodEase;
        public float _TurnOffDelaySeconds = 1f;
        [SerializeField] private bool _UseAutoMerge;
        [ShowIf("_UseAutoMerge")][SerializeField] private float _StartAutoMergeDelayDuration = 3;
        [SerializeField] private float _MoveToMergeCenterDuration = 2;
        [SerializeField] private PunchData _MergingPunchData;
        public UltEvent _WhenStartMerge;
        public UltEvent _WhenStopMerge;
        public UltEvent _WhenMerge;

        [Header("Charging")]
        public float _DelayToCharge = 2;
        public float _StartChargingWaitTime = 8;
        public float _StartChargingDistance = 10;
        [SerializeField] private bool _InvertStartEndScale = false;
        [SerializeField] private Vector3 _StartChargingScale = Vector3.one;
        [SerializeField] private Vector3 _EndChargingScale = new Vector3(.3f, .3f, .3f);
        [SerializeField] private float _StartChargingDuration = .5f;
        [SerializeField] private float _RequireChargingDuration = 3f;
        [SerializeField] private Vector2 _PunchChargingBallPeriod = new Vector2( 0.3f, 0.1f);
        [SerializeField] private PunchData _ChargingPunchData;
        public UltEvent _WhenActivateChargingBall;
        public UltEvent _WhenCharging;

        [Header("Shooting")]
        [SerializeField] private float _ShootingDuration = 5f;
        public UltEvent _WhenStartShooting;
        public UltEvent _WhenShooting;

        [Header("End Shooting")]
        [SerializeField] private float _EndShootingHapticDuration;
        [SerializeField] private AnimationCurve _EndShootingHapticEase;
        public UltEvent _WhenEndShooting;

        public bool _ForceShoot = false;

        [FoldoutGroup("HandMeshShaking")] [SerializeField] private float _HandShakingStrength = 0.05f;
        [FoldoutGroup("HandMeshShaking")] [SerializeField] private float _HandShakingDuration = 0.1f;
        [FoldoutGroup("CameraShaking")][SerializeField] private float _CameraShakingStrength = 0.05f;
        [FoldoutGroup("CameraShaking")][SerializeField] private float _CameraShakingDuration = 0.1f;
        [FoldoutGroup("Haptic")] [SerializeField] private string _GainBallHapticName;
        [FoldoutGroup("Haptic")] [SerializeField] private string _BallMergingPopHapticName;
        [FoldoutGroup("Haptic")] [SerializeField] private string _GainChargingBallHapticName;
        [FoldoutGroup("Haptic")] [SerializeField] private string _ChargingHapticName;
        [FoldoutGroup("Haptic")] [SerializeField] private string _ShootingHapticName;
        [FoldoutGroup("Audio")] [SerializeField] private string _GainBallAudioName;
        [FoldoutGroup("Audio")] [SerializeField] private string _BallMergingPopAudioName;
        [FoldoutGroup("Audio")] [SerializeField] private string _BallStopMergingAudioName;
        [FoldoutGroup("Audio")] [SerializeField] private string _GainChargingBallAudioName;
        [FoldoutGroup("Audio")] [SerializeField] private string _ChargingAudioName;
        [FoldoutGroup("Audio")] [SerializeField] private string _ShootingAudioName;
        [FoldoutGroup("Audio")] [SerializeField] private string _EndShootingAudioName;
        
        private PlayerData _PlayerData;
        private List<Vector3> energyObjectsOriginalPos = new List<Vector3>();
        private Vector3 beamControllerOriginalScale;
        private int _GainEnergyCount = 2;
        private int currentEnergyCount;
        private float CurrentStayTime;
        private float CurrentPeriod;
        private Vector3 originalEnergyballScale;
        private Vector3 centerOfEnergy = Vector3.zero;
        private Coroutine CheckDistanceProcess;
        private Coroutine CheckAutoMergeProcess;
        private Coroutine ChargingBallPositioningProcess;
        private Coroutine ChargingBallProcess;
        private Coroutine ShootingProcess;

        private void Awake()
        {
            if(!_ForceShoot) 
                _PlayersHands = GameObject.FindGameObjectsWithTag(_PlayersHandTag);
            if (GameObject.FindGameObjectWithTag(_PlayerTag).TryGetComponent<PlayerData>(out var playerData)) _PlayerData = playerData;
            _LineRendererDrawer.gameObject.SetActive(false);
            originalEnergyballScale = _PickableEnergys[0]._Energy.transform.localScale;
            beamControllerOriginalScale = _BeamChargeController.transform.localScale;            
            foreach (var energy in _PickableEnergyObjects)
                energyObjectsOriginalPos.Add(energy.transform.position);

            GetVibratorRef();
        }
        private void GetVibratorRef() {
            ControllerVibrator vibrator = null;
            for (int i = 0; i < _PlayersHands.Length; i++)
            {
                vibrator = _PlayersHands[i].GetComponentInParent<ControllerVibrator>();

                if (vibrator != null)
                {
                    if (vibrator.handType == HandType.Left)
                        _ControllerVibratorL = vibrator;
                    else if (vibrator.handType == HandType.Right)
                        _ControllerVibratorR = vibrator;
                }
            }
        }
        private void OnEnable()
        {
            ResetToBeggingState();

            foreach (var energy in _PickableEnergys)
            {
                energy.OnPlayerGetEnergy += StoreEnergyOnHand;
            }

            if (_UseAutoGrab)
                AutoGrabSequencer();

            _WhenStartMerge += () => { ShakeHandMesh(); };
            _WhenStopMerge += () => { ShakeHandMesh(); };
            _WhenShooting += () => { ShakeHandMesh(); DoCameraShake(); };
            _WhenActivateChargingBall += () => { ShakeHandMesh(); DoCameraShake(); };
        }
        private void OnDisable()
        {
            foreach (var energy in _PickableEnergys)
            {
                energy.OnPlayerGetEnergy -= StoreEnergyOnHand;
            }

            _WhenStartMerge -= () => { ShakeHandMesh(); };
            _WhenStopMerge -= () => { ShakeHandMesh(); };
            _WhenShooting -= () => { ShakeHandMesh(); DoCameraShake(); };
            _WhenActivateChargingBall -= () => { ShakeHandMesh(); DoCameraShake(); };
        }        
        private void Update()
        {
            centerOfEnergy = GetTwoHandsCenter();

            if (_CarryTheseVFXs.Length == 0) return;

            for (int i = 0; i < _CarryTheseVFXs.Length; i++)
            {
                _CarryTheseVFXs[i].transform.position = centerOfEnergy;
            }
        }
        private void ResetToBeggingState()
        {
            for (int i = 0; i < _PickableEnergyObjects.Length; i++)
            {
                _PickableEnergyObjects[i].transform.position = energyObjectsOriginalPos[i];
                _PickableEnergyObjects[i].gameObject.SetActive(true);
                print(_PickableEnergyObjects[i].gameObject.name + " " + _PickableEnergyObjects[i].gameObject.activeSelf);                
            }

            _BeamChargeController.controller = Player_BeamCharge_Controller.Controller.Start;
            _BeamChargeController.gameObject.SetActive(false);
            _BeamChargeController.transform.localScale = beamControllerOriginalScale;
        }
        private Vector3 GetTwoHandsCenter() {
            return (_PickableEnergys[0]._Energy.position + _PickableEnergys[1]._Energy.position) / 2;
        }
        private void ShakeHandMesh() {
            _PlayerData.lHandShaker.StartShake(_HandShakingStrength, _HandShakingDuration); 
            _PlayerData.rHandShaker.StartShake(_HandShakingStrength, _HandShakingDuration);
        }
        private void DoCameraShake() {
            _PlayerData.cameraShakeEvent.OnEnable();
            _PlayerData.cameraShakeSettings.StartShaking(_CameraShakingDuration, _CameraShakingStrength, 10);
        }
        #region Pick Energy
        private void AutoGrabSequencer() {
            if (_ForceShoot) { 
                StartCoroutine(HardFixSequencer());
                return;
            }
            
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(_AutoGrabDelayDuration);
            TweenCallback autograbCallback = () => { 
                AutoGrabEnergy();                              
            };
            seq.AppendCallback(autograbCallback);
            seq.Play();            
        }
        private IEnumerator HardFixSequencer() {
            yield return new WaitForSeconds(_AutoGrabDelayDuration);
            AutoGrabEnergy();
            yield return new WaitForSeconds(_AutoGrabDuration); 
            StartMerging();
            yield return new WaitForSeconds(_StartCombineDuration);
            StartCoroutine(TurnOffDelayer());
            _BeamChargeController.gameObject.SetActive(true);
            ChargingBall();
            yield return new WaitForSeconds(_DelayToCharge + _RequireChargingDuration);
            ChargingBallPositioningProcess = StartCoroutine(ChargingBallPositioning());
            StartShooting();
            _WhenStartShooting?.Invoke();
            yield return new WaitForSeconds(_ShootingDuration * 1.5f);
            EndShooting();
            _WhenEndShooting?.Invoke();
        }
        public void AutoGrabEnergy() {
            print("Auto Grab Energy");
            foreach (var energy in _PickableEnergys)
            {
                Transform targetHand;
                Vector3  targetHandPos = Vector3.zero;

                if (energy._HandType == HandType.Left)
                    targetHand = _PlayersHands[0].transform;
                else
                    targetHand = _PlayersHands[1].transform;

                targetHandPos = targetHand.position;

                Sequence seq = DOTween.Sequence();
                seq.Append(
                energy.transform.DOMove(targetHandPos, _AutoGrabDuration).SetEase(_AutoGrabEase).OnUpdate(() => {
                    targetHandPos = targetHand.position;
                }));
                seq.AppendInterval(_AutoGrabDelayDuration);
                TweenCallback storeEnergyCallback = () => {
                    print("Check Store Energy");
                    if (targetHand.TryGetComponent<InteractCollider>(out var handCol))
                        StoreEnergyOnHand(energy, handCol);                       
                };
                seq.AppendCallback(storeEnergyCallback);
            }
        }
        private void StoreEnergyOnHand(PickableEnergy energy, InteractCollider handCol)
        {
            print("Store Energy");
            currentEnergyCount++;

            if (currentEnergyCount >= _GainEnergyCount)
                StartMerging();
            
            DoVibration(handCol._HandType, _GainBallHapticName);
            AudioPlayerHelper.PlayAudio(_GainBallAudioName, transform.position);
        }
        #endregion
        #region Merging Energy
        private void StartMerging() {
            print("Start Merging");
            _LineRendererDrawer._From = _PickableEnergys[0]._Energy;
            _LineRendererDrawer._To = _PickableEnergys[1]._Energy;
            _LineRendererDrawer.gameObject.SetActive(true);

            if (CheckDistanceProcess != null)
                StopCoroutine(CheckDistanceProcess);

            CheckDistanceProcess = StartCoroutine(CheckMerging());

            if (!_UseAutoMerge) return;

            if (CheckAutoMergeProcess != null)
                StopCoroutine(CheckAutoMergeProcess);

            CheckAutoMergeProcess = StartCoroutine(CheckAutoMerge());
        }
        private IEnumerator CheckAutoMerge() {
            float timer = 0;
            
            while (timer < _StartAutoMergeDelayDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            var startTime = Time.time;
            var endTime = _MoveToMergeCenterDuration;
            var step = (Time.time - startTime) / endTime;
            var center = GetTwoHandsCenter();

            while (Time.time - startTime < endTime)
            {
                step = (Time.time - startTime) / endTime;
                center = GetTwoHandsCenter();
                _PickableEnergys[0]._Energy.position = Vector3.Lerp(_PickableEnergys[0]._Energy.position, center, step);
                _PickableEnergys[1]._Energy.position = Vector3.Lerp(_PickableEnergys[1]._Energy.position, center, step);
                yield return null;
            }

            while (true)
            {
                center = GetTwoHandsCenter();
                _PickableEnergys[0]._Energy.position = center;
                _PickableEnergys[1]._Energy.position = center;
                yield return null;
            }
        }
        private IEnumerator CheckMerging() {
            yield return new WaitUntil(() => CheckStayTiming(CheckStartCombineDistance()));
            
            if (CheckAutoMergeProcess != null)
                StopCoroutine(CheckAutoMergeProcess);

            _BeamChargeController.gameObject.SetActive(true);

            _WhenActivateChargingBall?.Invoke();
            
            StartCoroutine(TurnOffDelayer());
            ChargingBall();
        }
        private IEnumerator TurnOffDelayer() {
            yield return new WaitForSeconds(_TurnOffDelaySeconds);
            foreach (var item in _PickableEnergys)
            {
                item._Energy.gameObject.SetActive(false);
            }
            foreach (var item in _PickableEnergyObjects)
            {
                item.SetActive(false);
            }
            _LineRendererDrawer.gameObject.SetActive(false);
        }
        private float EnergyDistance() => Vector3.Distance(_PickableEnergys[0]._Energy.position, _PickableEnergys[1]._Energy.position);
        private bool CheckStartCombineDistance() => EnergyDistance() < _StartCombineDistance;
        private bool CheckStayTiming(bool distanceResult) {
            if (!distanceResult) {
                if (CurrentStayTime != 0)
                { 
                    AudioPlayerHelper.PlayAudio(_BallStopMergingAudioName, transform.position); 
                    _WhenStopMerge?.Invoke();                    
                }
                CurrentStayTime = 0;
                ResetEnergyScale();
                return false; 
            }
            
            if (CurrentStayTime == 0)
            {
                FeedbackPopEnergys();
                _WhenStartMerge?.Invoke();
            }

            CurrentStayTime += Time.deltaTime;

            CurrentPeriod = Mathf.Lerp(_PunchEnergyPeriod.x, _PunchEnergyPeriod.y, _PeriodEase.Evaluate(CurrentStayTime / _StartCombineDuration));

            
            if (CurrentStayTime % CurrentPeriod <= .01f)
            { 
                FeedbackPopEnergys(); 
                _WhenMerge?.Invoke(); 
            }

            if (CurrentStayTime >= _StartCombineDuration) return true;
            else return false;
        }
        private void ResetEnergyScale() {
            for (int i = 0; i < _PickableEnergys.Length; i++)
            {
                _PickableEnergys[i]._Energy.transform.DOScale(originalEnergyballScale, _MergingPunchData.PunchDuration);
            }
        }
        private void FeedbackPopEnergys() {
            for (int i = 0; i < _PickableEnergys.Length; i++)
            {
                _PickableEnergys[i]._Energy.transform.DOPunchScale(_MergingPunchData.PunchScale, _MergingPunchData.PunchDuration, _MergingPunchData.Vibrato);
                DoVibration(HandType.Left, _BallMergingPopHapticName);
                DoVibration(HandType.Right, _BallMergingPopHapticName);
                AudioPlayerHelper.PlayAudio(_BallMergingPopAudioName, transform.position);
            }
        }
        #endregion
        #region ChargingBall
        private void ChargingBall() {
            if (ChargingBallProcess != null)
                StopCoroutine(ChargingBallProcess);

            ChargingBallPositioningProcess = StartCoroutine(ChargingBallPositioning());
            ChargingBallProcess = StartCoroutine(ChargingBallControlling());

            DoVibration(HandType.Left, _GainChargingBallHapticName);
            DoVibration(HandType.Right, _GainChargingBallHapticName);
            AudioPlayerHelper.PlayAudio(_GainChargingBallAudioName, transform.position);
        }
        private IEnumerator ChargingBallPositioning()
        {
            while (true)
            {
                _BeamChargeController.transform.position = GetTwoHandsCenter();
                yield return null;
            }
        }
        private IEnumerator ChargingBallControlling() {
            float waitDuration = 0;            
            float currentDuration = 0;            

            _BeamChargeController.transform.DOScale(_StartChargingScale, _StartChargingDuration);

            yield return new WaitForSeconds(_DelayToCharge);

            print("Start Charging");

            while (currentDuration < _RequireChargingDuration) {

                waitDuration += Time.deltaTime;

                if (EnergyDistance() < _StartChargingDistance || waitDuration >= _StartChargingWaitTime)
                {
                    currentDuration += Time.deltaTime;

                    CurrentPeriod = Mathf.Lerp(_PunchChargingBallPeriod.x, _PunchChargingBallPeriod.y, currentDuration / _RequireChargingDuration);

                    if (currentDuration % CurrentPeriod <= 0.01f)
                    {
                        _BeamChargeController.transform.DOPunchScale(_ChargingPunchData.PunchScale, _ChargingPunchData.PunchDuration, _ChargingPunchData.Vibrato);

                        DoVibration(HandType.Left, _ChargingHapticName);
                        DoVibration(HandType.Right, _ChargingHapticName);
                        AudioPlayerHelper.PlayAudio(_ChargingAudioName, transform.position);
                    }
                    else
                    {
                        if (!_InvertStartEndScale)
                            _BeamChargeController.transform.localScale = Vector3.Lerp(_StartChargingScale, _EndChargingScale, currentDuration / _RequireChargingDuration);
                        else
                            _BeamChargeController.transform.localScale = Vector3.Lerp(_EndChargingScale, _StartChargingScale, currentDuration / _RequireChargingDuration);
                    }
                }

                yield return null; 
            }

            if (!_InvertStartEndScale)
                _BeamChargeController.transform.localScale = _EndChargingScale;
            else
                _BeamChargeController.transform.localScale = _StartChargingScale;

            print("Finished Charging");
            StartShooting();
        }
        #endregion
        private void StartShooting() {
            _BeamChargeController.controller = Player_BeamCharge_Controller.Controller.Shot;

            if (ShootingProcess != null)
                StopCoroutine(ShootingProcess);

            ShootingProcess = StartCoroutine(Shooting());
        }
        private IEnumerator Shooting()
        {
            DoDynamicVibration(HandType.Left, _ShootingHapticName, 1, 1, 0.1f, _ShootingDuration, _EndShootingHapticEase);
            DoDynamicVibration(HandType.Right, _ShootingHapticName, 1, 1, 0.1f, _ShootingDuration, _EndShootingHapticEase);
            AudioPlayerHelper.PlayAudio(_ShootingAudioName, transform.position);
            _WhenStartShooting?.Invoke();
            var feedbackProcess = StartCoroutine(ShootingFeedback());
            yield return new WaitForSeconds(_ShootingDuration);
            StopCoroutine(feedbackProcess);
            DoDynamicVibration(HandType.Left, _ShootingHapticName, 1, 0, 0, _EndShootingHapticDuration, _EndShootingHapticEase);
            DoDynamicVibration(HandType.Right, _ShootingHapticName, 1, 0, 0, _EndShootingHapticDuration, _EndShootingHapticEase);
            AudioPlayerHelper.PlayAudio(_EndShootingAudioName, transform.position);
            _WhenEndShooting?.Invoke();
            EndShooting();
        }
        private IEnumerator ShootingFeedback() {
            while (true)
            {
                _WhenShooting?.Invoke();
                yield return new WaitForSeconds(.1f);
            }
        }

        private void EndShooting() { 
            _BeamChargeController.controller = Player_BeamCharge_Controller.Controller.End;            
        }
        private ControllerVibrator GetHandVibrator(HandType hand) {
            if (hand == HandType.Left)
                return _ControllerVibratorL;
            else 
                return _ControllerVibratorR;
        }
        private void DoVibration(HandType hand, string vibrationName) {
            //GetHandVibrator(hand).VibrateWithSetting(vibrationName);
        }        
        private void DoDynamicVibration(HandType hand, string vibrationName, float from, float to, float period, float duration, AnimationCurve ease) {
            var controllerVibrator = GetHandVibrator(hand);
            var setting = controllerVibrator.FindSettings(vibrationName);

            float step = from;

            DOTween.To(() => step, x => step = x, to, duration).SetEase(ease).OnUpdate(() => {
                if (step % period <= 0.1f || period == 0) controllerVibrator.DynamicVibrateWithSetting(setting, step);
            });
        }
    }
}
