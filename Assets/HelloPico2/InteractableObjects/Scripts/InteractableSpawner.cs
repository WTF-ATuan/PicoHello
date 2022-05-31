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
        public int _MaxAmount = 5;
        public float _RefillDelayDuration;
        public List<GameObject> _Clonelist = new List<GameObject>();

        [Header("Gizmos Settings")]
        public Color _DrawColor;

        private void Start()
        {
            for (int i = 0; i < _MaxAmount; i++)
            {
                var pos = _SpawnPosition.position + _SpawnPosition.transform.right * i * _SpawnOffset;
                var clone = SpawnObject(pos);
                _Clonelist.Add(clone);
            }
        }
        private void OnDisable()
        {
            foreach (var obj in _Clonelist) {
                if (obj.TryGetComponent<InteractableBase>(out var interactable)) {
                    interactable.OnInteractableDisable -= UpdateWhenDestroy; 
                }
            }
        }
        public GameObject SpawnObject(Vector3 spawnPos) {
            var clone = Instantiate(_SpawnThis, transform);

            clone.transform.position = spawnPos;
            
            if (clone.TryGetComponent<InteractableBase>(out var interactable))
            {
                interactable.OnInteractableDisable += UpdateWhenDestroy;                
            }

            return clone;
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
                //Gizmos.DrawWireSphere(pos, _SpawnThis.transform.localScale.x);
                Gizmos.DrawWireMesh(_SpawnThis.GetComponent<MeshFilter>().sharedMesh, pos, Quaternion.identity, _SpawnThis.transform.localScale);
            }
        }
    }
}
