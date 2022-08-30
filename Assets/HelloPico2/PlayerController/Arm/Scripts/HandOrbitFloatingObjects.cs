using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HelloPico2.PlayerController
{
    public class HandOrbitFloatingObjects : MonoBehaviour
    {
        [SerializeField] private Transform[] _OrbitPos;
        [SerializeField] private float _ScaleMultiplier = .3f;
        [SerializeField] private Vector3 _Rotation;        
        [SerializeField] private float _ShowUpPunch = 1.3f;
        [SerializeField] private float _ShowUpPunchDuration = .5f;
        [SerializeField] private float _HideDuration = 1;
        [SerializeField] private Ease __HideEase;

        [SerializeField] private GameObject _FloatingBombPrefab;

        [SerializeField] private int currentObj = -1;
        [SerializeField] private List<GameObject> cloneList;
        public void AddBomb() {
            AddOrbitObject(_FloatingBombPrefab);
        }
        public void AddOrbitObject(GameObject obj) {
            if (currentObj >= _OrbitPos.Length - 1) return;
            
            currentObj++;

            var clone = Instantiate(obj, _OrbitPos[currentObj]);
            clone.transform.SetParent(_OrbitPos[currentObj], false);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localEulerAngles = _Rotation;
            clone.transform.localScale *= _ScaleMultiplier;
            clone.transform.DOPunchScale(_ShowUpPunch * Vector3.one, _ShowUpPunchDuration, 2);

            cloneList.Add(clone);
        }
        public void RemoveOrbitObject()
        {
            if (cloneList.Count == 0) return;

            cloneList[currentObj].transform.DOScale(Vector3.zero, _HideDuration).From(cloneList[currentObj].transform.localScale).SetEase(__HideEase);

            cloneList.Remove(cloneList[currentObj]);

            currentObj--;
        }
    }
}
