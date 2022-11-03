

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class InteractableSpawner : MonoBehaviour
    {
        public GameObject _SpawnThis;
        public Transform _SpawnPosition;
        public float _SpawnOffset;
        public Vector3 _SpawnRotation;
        public int _MaxAmount = 5;
        public float _LifeTime = 10;
        public float _RefillDelayDuration;
        public List<GameObject> _Clonelist = new List<GameObject>();

        [Header("Loop Settings")]
        public bool _UseWaveModule = false;
        [Min(1)]public int _SpawnWavesAmount = 1;
        public float _WaveDelayDuration = 5f;

        [Header("Timed Refill")]
        public bool _UseTimedRefill = false;

        [Header("Moving")]
        public bool _UseMovingModule = false;
        public Vector3 _MoveDirection;
        public float _MoveSpeed;

        [Header("Gizmos Settings")]
        public Color _DrawColor;

        private Coroutine WaveProcess;

        private void SpawnSingleWave() {
            for (int i = 0; i < _MaxAmount; i++)
            {
                var pos = _SpawnPosition.position + _SpawnPosition.transform.right * i * _SpawnOffset;
                var clone = SpawnObject(pos);
                clone.transform.eulerAngles = _SpawnRotation;
                _Clonelist.Add(clone);
            }

            if (_UseTimedRefill)
                StartCoroutine(CheckCloneList());
        }
        private IEnumerator CheckCloneList() {
            while (_Clonelist.Count > 0)
            {
                for (int i = 0; i < _Clonelist.Count; i++)
                {
                    if (_Clonelist[i] == null)
                    {
                        var pos = _SpawnPosition.position + _SpawnPosition.transform.right * i * _SpawnOffset;
                        var clone = SpawnObject(pos);
                        clone.transform.eulerAngles = _SpawnRotation;
                        _Clonelist[i] = clone;
                    }
                }
                yield return new WaitForSeconds(_RefillDelayDuration);
            }
        }
        private IEnumerator WaveControl() {
            for (int j = 0; j < _SpawnWavesAmount; j++)
            {
                SpawnSingleWave();
                yield return new WaitForSeconds(_WaveDelayDuration);
            }
        }

        private void OnEnable(){
            if(_Clonelist.Count >= 1) return;

            if (_UseWaveModule)
            {
                if (WaveProcess != null)
                    StopCoroutine(WaveProcess);

                WaveProcess = StartCoroutine(WaveControl()); 
            }
            else
                SpawnSingleWave();
        }

        private void OnDisable()
        {
            foreach (var obj in _Clonelist) {
                if (obj == null || !obj.TryGetComponent<InteractableBase>(out var interactable)) continue;
                    
                interactable.OnInteractableDisable -= UpdateWhenDestroy;
                Destroy(obj);
            }
            _Clonelist.Clear();
        }
        public GameObject SpawnObject(Vector3 spawnPos) {
            var clone = Instantiate(_SpawnThis, transform);

            clone.transform.position = spawnPos;
            
            if (clone.TryGetComponent<InteractableBase>(out var interactable))
            {
                interactable.OnInteractableDisable += UpdateWhenDestroy;                         
            }

            if (_UseMovingModule) AddMovingModule(clone, interactable);

            return clone;
        }
        private void AddMovingModule(GameObject clone, InteractableBase interactable) {
            var moveObject = clone.AddComponent<MoveObject>();
            moveObject.relativeDirection = _MoveDirection;
            moveObject.speed = _MoveSpeed;

            if(interactable)
                interactable.OnInteractableDisable += (InteractableBase interactable) => Destroy(clone.GetComponent<MoveObject>());
        }
        private void UpdateWhenDestroy(InteractableBase interactable) {
            if (_Clonelist.Count <= _MaxAmount)
            {
                var index = _Clonelist.IndexOf(interactable.gameObject);
                var pos = _SpawnPosition.position + _SpawnPosition.transform.right * index * _SpawnOffset;
                var clone = SpawnObject(pos);
                _Clonelist[index] = clone;

                StartCoroutine(DelayToRefill(clone));
            }
        }
        private IEnumerator DelayToRefill(GameObject obj) {
            obj.SetActive(false);
            yield return new WaitForSeconds(_RefillDelayDuration);
            obj.SetActive(true);
        }

        private void OnDrawGizmos()
        {
            if (_SpawnThis == null) return;

            for (int i = 0; i < _MaxAmount; i++)
            {
                var pos = _SpawnPosition.position + _SpawnPosition.transform.right * i * _SpawnOffset;
                
                Gizmos.color = _DrawColor;

                if(!_SpawnThis.TryGetComponent<MeshFilter>(out var mesh)) return;

                var sharedMesh = mesh.sharedMesh;
                var childMesh = _SpawnThis.GetComponentInChildren<MeshFilter>().sharedMesh;

                Gizmos.DrawWireMesh(!sharedMesh ? childMesh : sharedMesh, pos, Quaternion.Euler(_SpawnRotation),
                    _SpawnThis.transform.localScale);
            }
        }
    }
}
