using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace HelloPico2.PlayerController
{
    public class HandOrbitFloatingObjects : SerializedMonoBehaviour
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
        public IOribitMovement movement;
        [SerializeField] private float _MovementTransitionDuration = .5f;
        bool hasGrabbingProcess;
        public void AddBomb()
        {
            if (!HasSlot() || hasGrabbingProcess) return;
            hasGrabbingProcess = true;
            var obj = GenerateOrbitObject(_FloatingBombPrefab);
            if (movement != null)
            {
                System.Action act = () => { AddOrbitObject(obj); };
                movement.Move(obj, act);
            }
            else {
                AddOrbitObject(obj);
            }
        }
        private GameObject GenerateOrbitObject(GameObject obj) {
            var clone = Instantiate(obj);
            clone.transform.localScale = Vector3.one * _ScaleMultiplier;
            cloneList.Add(clone);
            return clone;
        }
        private void AddOrbitObject(GameObject obj) {            
            currentObj++;

            obj.transform.SetParent(_OrbitPos[currentObj]);
            obj.transform.DOLocalMove(Vector3.zero, _MovementTransitionDuration);
            obj.transform.DOLocalRotate(_Rotation, _MovementTransitionDuration).OnComplete(() => { 
                obj.transform.DOPunchScale(_ShowUpPunch * Vector3.one, _ShowUpPunchDuration, 2);
                hasGrabbingProcess = false;
            });
        }
        public void RemoveOrbitObject()
        {
            if (cloneList.Count == 0) return;

            cloneList[currentObj].transform.DOScale(Vector3.zero, _HideDuration).From(cloneList[currentObj].transform.localScale).SetEase(__HideEase);

            cloneList.Remove(cloneList[currentObj]);

            currentObj--;
        }
        private bool HasSlot() {
            return currentObj < _OrbitPos.Length - 1;
        }
    }
}
