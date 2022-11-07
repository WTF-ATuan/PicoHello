using DG.Tweening;
using HelloPico2.PlayerController.Arm;
using Sirenix.OdinInspector;
using System.Collections;
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
        public LineRendererDrawer _LineRendererDrawer;
        public Player_BeamCharge_Controller _BeamChargeController;
        
        [Header("Merging")]
        public float _StartCombineDistance = 10;
        public float _StartCombineDuration = 3;
        public Vector2 _PunchEnergyPeriod = new Vector2( 0.3f, 0.1f);
        public AnimationCurve _PeriodEase;
        public float _TurnOffDelaySeconds = 1f;
        [SerializeField] private PunchData _MergingPunchData;

        [Header("Charging")]
        public float _StartChargingDistance = 10;
        [SerializeField] private bool _InvertStartEndScale = false;
        [SerializeField] private Vector3 _StartChargingScale = Vector3.one;
        [SerializeField] private Vector3 _EndChargingScale = new Vector3(.3f, .3f, .3f);
        [SerializeField] private float _StartChargingDuration = .5f;
        [SerializeField] private float _RequireChargingDuration = 3f;
        [SerializeField] private Vector2 _PunchChargingBallPeriod = new Vector2( 0.3f, 0.1f);
        [SerializeField] private PunchData _ChargingPunchData;

        [Header("Shooting")]
        [SerializeField] private float _ShootingDuration = 5f;

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
            
            StartCoroutine(TurnOffDelayer());
            ChargingBall();
        }
        private IEnumerator TurnOffDelayer() {
            yield return new WaitForSeconds(_TurnOffDelaySeconds);
            foreach (var item in _PickableEnergys)
            {
                item._Energy.gameObject.SetActive(false);
            }
            _LineRendererDrawer.gameObject.SetActive(false);
        }
        private float EnergyDistance() => Vector3.Distance(_PickableEnergys[0]._Energy.position, _PickableEnergys[1]._Energy.position);
        private bool CheckStartCombineDistance() => EnergyDistance() < _StartCombineDistance;
        private bool CheckStayTiming(bool distanceResult) {
            if (!distanceResult) {
                CurrentStayTime = 0;
                ResetEnergyScale();
                return false; 
            }

            CurrentStayTime += Time.deltaTime;

            CurrentPeriod = Mathf.Lerp(_PunchEnergyPeriod.x, _PunchEnergyPeriod.y, _PeriodEase.Evaluate(CurrentStayTime / _StartCombineDuration));

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
            }
        }
        #endregion
        #region ChargingBall
        private void ChargingBall() {
            if (ChargingBallProcess != null)
                StopCoroutine(ChargingBallProcess);

            ChargingBallPositioningProcess = StartCoroutine(ChargingBallPositioning());
            ChargingBallProcess = StartCoroutine(ChargingBallControlling());
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
            float currentDuration = 0;            

            _BeamChargeController.transform.DOScale(_StartChargingScale, _StartChargingDuration);

            while (currentDuration < _RequireChargingDuration) {                

                if (EnergyDistance() < _StartChargingDistance)
                {
                    currentDuration += Time.deltaTime;

                    CurrentPeriod = Mathf.Lerp(_PunchChargingBallPeriod.x, _PunchChargingBallPeriod.y, currentDuration / _RequireChargingDuration);

                    if (currentDuration % CurrentPeriod <= 0.01f)
                        _BeamChargeController.transform.DOPunchScale(_ChargingPunchData.PunchScale, _ChargingPunchData.PunchDuration, _ChargingPunchData.Vibrato);
                    else
                    { 
                        if(!_InvertStartEndScale)
                            _BeamChargeController.transform.localScale = Vector3.Lerp(_StartChargingScale, _EndChargingScale, currentDuration / _RequireChargingDuration); 
                        else
                            _BeamChargeController.transform.localScale = Vector3.Lerp(_EndChargingScale, _StartChargingScale,  currentDuration / _RequireChargingDuration); 
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
        private IEnumerator Shooting() {
            yield return new WaitForSeconds(_ShootingDuration);
            EndShooting();
        }

        private void EndShooting() { 
            _BeamChargeController.controller = Player_BeamCharge_Controller.Controller.End;            
        }
    }
}
