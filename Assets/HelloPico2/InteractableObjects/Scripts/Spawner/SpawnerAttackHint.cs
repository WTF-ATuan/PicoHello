using System;
using DG.Tweening;
using HelloPico2.LevelTool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace HelloPico2.InteractableObjects
{
    public class SpawnerAttackHint : MonoBehaviour
    {
        public GameObject _AttackSignVFX;
        public float _VFXScale = 1;
        public float _EnableDelayDuration = 0.1f;
        public EmissionRaiseSteps _EmissionControl;
        public float _EmissionDelayDuration = 0.1f;
        public bool _Flipflop = true;
        public float _Duration = 0.3f;
        private ISpawner _spawner;

        private void OnEnable()
        {
            _spawner = GetComponent<ISpawner>();
            if (_spawner == null) throw new Exception($"Can,t Get Spawner in {gameObject}");
            _spawner.OnSpawn += AddAttackSign;
            _spawner.OnSpawn += AddEmissionControl;
        }

        private void OnDisable()
        {
            _spawner.OnSpawn -= AddAttackSign;
            _spawner.OnSpawn -= AddEmissionControl;
        }
        private void AddAttackSign(GameObject obj) { 
            var clone = Instantiate(_AttackSignVFX, obj.transform);
            clone.SetActive(false);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localScale *= _VFXScale;

            if (clone.TryGetComponent<EnableEvent>(out var enableEvent))
                enableEvent.m_DelayTime = _EnableDelayDuration;

            if (obj.TryGetComponent<IDestroyChecker>(out var iDestroyChecker))            
                iDestroyChecker.OnDestroy += (HitTargetObj) => { clone.SetActive(false); };            

            clone.SetActive(true);
        }
        private void AddEmissionControl(GameObject obj) {
            var emissionControl = obj.AddComponent<EmissionRaiseSteps>();
            emissionControl.enabled = false;
            emissionControl.AssignValue(_EmissionControl);
            emissionControl.TargetRenderer = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(_EmissionDelayDuration);
            seq.AppendCallback(() => { 
                emissionControl.RaiseToColor(1, false, _Flipflop, _Duration); 
            });
            seq.Play();

            emissionControl.enabled = true;
        }
    }
}
