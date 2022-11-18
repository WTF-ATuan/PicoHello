using DG.Tweening;
using HelloPico2.InputDevice;
using HelloPico2.InputDevice.Scripts;
using HelloPico2.PlayerController.Arm;
using Sirenix.OdinInspector;
using System.Collections;
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
        public string _PlayersHandTag = "Interactable";
        [ReadOnly] public GameObject[] _PlayersHands;
        public PickableEnergy[] _PickableEnergys;
        public GameObject[] _PickableEnergyObjects;
        public LineRendererDrawer _LineRendererDrawer;
        public Player_BeamCharge_Controller _BeamChargeController;
        [ReadOnly][SerializeField] private ControllerVibrator _ControllerVibratorL, _ControllerVibratorR;
        
        [Header("Merging")]
        public float _StartCombineDistance = 10;
        public float _StartCombineDuration = 3;
        public Vector2 _PunchEnergyPeriod = new Vector2( 0.3f, 0.1f);
        public AnimationCurve _PeriodEase;
        public float _TurnOffDelaySeconds = 1f;
        [SerializeField] private PunchData _MergingPunchData;

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

        [Header("Shooting")]
        [SerializeField] private float _ShootingDuration = 5f;
        public UltEvent _WhenStartShooting;

        [Header("End Shooting")]
        [SerializeField] private float _EndShootingHapticDuration;
        [SerializeField] private AnimationCurve _EndShootingHapticEase;
        public UltEvent _WhenEndShooting;

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

        private int _GainEnergyCount = 2;
        private int currentEnergyCount;
        private float CurrentStayTime;
        private float CurrentPeriod;
        private Vector3 originalEnergyballScale;
        private Coroutine CheckDistanceProcess;
        private Coroutine ChargingBallPositioningProcess;
        private Coroutine ChargingBallProcess;
        private Coroutine ShootingProcess;

        private void Awake()
        {
            _PlayersHands = GameObject.FindGameObjectsWithTag(_PlayersHandTag);
            _LineRendererDrawer.gameObject.SetActive(false);
            originalEnergyballScale = _PickableEnergys[0]._Energy.transform.localScale;

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
            foreach (var energy in _PickableEnergys)
            {
                energy.OnPlayerGetEnergy += StoreEnergyOnHand;
            }
        }
        private void OnDisable()
        {
            foreach (var energy in _PickableEnergys)
            {
                energy.OnPlayerGetEnergy -= StoreEnergyOnHand;
            }
        }
        #region Pick Energy
        private void StoreEnergyOnHand(PickableEnergy energy, InteractCollider handCol)
        {
            currentEnergyCount++;

            if (currentEnergyCount == _GainEnergyCount)
                StartMerging();
            
            DoVibration(handCol._HandType, _GainBallHapticName);
            AudioPlayerHelper.PlayAudio(_GainBallAudioName, transform.position);
        }
        #endregion
        #region Merging Energy
        private void StartMerging() {
            _LineRendererDrawer._From = _PickableEnergys[0]._Energy;
            _LineRendererDrawer._To = _PickableEnergys[1]._Energy;
            _LineRendererDrawer.gameObject.SetActive(true);

            if (CheckDistanceProcess != null)
                StopCoroutine(CheckDistanceProcess);

            CheckDistanceProcess = StartCoroutine(CheckMerging());
        }
        private IEnumerator CheckMerging() {
            yield return new WaitUntil(() => CheckStayTiming(CheckStartCombineDistance()));
            
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
                if(CurrentStayTime != 0) AudioPlayerHelper.PlayAudio(_BallStopMergingAudioName, transform.position);
                CurrentStayTime = 0;
                ResetEnergyScale();
                return false; 
            }

            CurrentStayTime += Time.deltaTime;

            CurrentPeriod = Mathf.Lerp(_PunchEnergyPeriod.x, _PunchEnergyPeriod.y, _PeriodEase.Evaluate(CurrentStayTime / _StartCombineDuration));
            
            if (CurrentStayTime == 0)
                FeedbackPopEnergys();
            if (CurrentStayTime % CurrentPeriod <= .01f)
                FeedbackPopEnergys();

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
                _BeamChargeController.transform.position = (_PickableEnergys[0]._Energy.position + _PickableEnergys[1]._Energy.position) / 2;

                yield return null;
            }
        }
        private IEnumerator ChargingBallControlling() {
            float waitDuration = 0;            
            float currentDuration = 0;            

            _BeamChargeController.transform.DOScale(_StartChargingScale, _StartChargingDuration);

            yield return new WaitForSeconds(_DelayToCharge);

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
            yield return new WaitForSeconds(_ShootingDuration); 
            DoDynamicVibration(HandType.Left, _ShootingHapticName, 1, 0, 0, _EndShootingHapticDuration, _EndShootingHapticEase);
            DoDynamicVibration(HandType.Right, _ShootingHapticName, 1, 0, 0, _EndShootingHapticDuration, _EndShootingHapticEase);
            AudioPlayerHelper.PlayAudio(_EndShootingAudioName, transform.position);
            _WhenEndShooting?.Invoke();
            EndShooting();
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
            GetHandVibrator(hand).VibrateWithSetting(vibrationName);
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
