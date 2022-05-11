using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class InteractableSpawner : MonoBehaviour
    {
        public GameObject _SpwanThis;
        public Transform _SpawnPosition;
        public float _SpawnOffset;
        public int _MaxAmount = 5;
        public float _RefillDelayDuration;
        public List<GameObject> _Clonelist = new List<GameObject>();

        private void Start()
        {
            for (int i = 0; i < _MaxAmount; i++)
            {
                var pos = _SpawnPosition.position + _SpawnPosition.transform.right * i * _SpawnOffset;
                var clone = SpawnObject(pos);
                _Clonelist.Add(clone);
            }
        }
        public GameObject SpawnObject(Vector3 spawnPos) {
            var clone = Instantiate(_SpwanThis, transform);

            clone.transform.position = spawnPos;
            
            clone.GetComponent<InteractableBase>().OnInteractableDisable += UpdateWhenDestroy;

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
    }
}
