using Dreamteck.Splines.Primitives;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PlayerController.BeamCharge
{
    public class SpiritLinker : MonoBehaviour
    {
        [ReadOnly][SerializeField] private List<GameObject> _Spirits = new List<GameObject>();
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private PlayerTwoHandBehavior _PlayerTwoHandBehavior;
        [SerializeField] private bool _ToTwoChargingBall;

        [SerializeField] private int _ActiveLineEachPeriod = 4;
        [SerializeField] private float _RefreshPeriod = 3;
        public bool toTwoChargingBall { get { return _ToTwoChargingBall; } set { _ToTwoChargingBall = value; } }

        [ReadOnly][SerializeField] private List<LineRenderer> lineRenderers = new List<LineRenderer>();
        [ReadOnly][SerializeField] private List<int> activeIndex;
        private Coroutine waveProcess;

        private void OnEnable()
        {
            GetSpirits();

            if(waveProcess == null)
                waveProcess = StartCoroutine(WaveControl());
        }
        private void OnDisable()
        {
            StopCoroutine(waveProcess);
        }
        private void Update()
        {
            for (int i = 0; i < _Spirits.Count; i++)
            {
                DrawLine(i);
            }
        }
        private void GetSpirits() {
            _Spirits = new List<GameObject>(GameObject.FindGameObjectsWithTag("Spirit"));            
        }
        private void CheckSpawnLine(int spiritIndex) {
            if (spiritIndex >= lineRenderers.Count)
                lineRenderers.Add(Instantiate(_lineRenderer, _Spirits[spiritIndex].transform));
        }
        private void DrawLine(int spiritIndex) {
            CheckSpawnLine(spiritIndex);

            lineRenderers[spiritIndex].SetPosition(0, _Spirits[spiritIndex].transform.position);
            if (_ToTwoChargingBall)
            {
                if (_Spirits[spiritIndex].transform.position.x >= 0)
                    lineRenderers[spiritIndex].SetPosition(1, _PlayerTwoHandBehavior._PickableEnergyObjects[0].transform.position); 
                else
                    lineRenderers[spiritIndex].SetPosition(1, _PlayerTwoHandBehavior._PickableEnergyObjects[1].transform.position); 
            }
            else
                lineRenderers[spiritIndex].SetPosition(1, _PlayerTwoHandBehavior._BeamChargeController.transform.position);
        }
        private IEnumerator WaveControl() {
            activeIndex = new List<int>();
            int pickIndex;

            while (true) {
                activeIndex.Clear();
                FadeOutAllLines();

                for (int i = 0; i < _ActiveLineEachPeriod; i++)
                {
                    pickIndex = -1;

                    while (activeIndex.Contains(pickIndex) || pickIndex == -1)
                    {
                        pickIndex = Random.Range(0, lineRenderers.Count);
                        yield return null;
                    }

                    FadeInLine(lineRenderers[pickIndex]);

                    activeIndex.Add(pickIndex);
                }

                yield return new WaitForSeconds(_RefreshPeriod);
            }
        }
        private void FadeInLine(LineRenderer line) { 
            line.enabled = true;        
        }
        private void FadeOutAllLines() {
            for (int i = 0; i < lineRenderers.Count; i++)
            {
                FadeOutLine(lineRenderers[i]);
            }
        }
        private void FadeOutLine(LineRenderer line) {
            line.enabled = false;
        }
    }
}