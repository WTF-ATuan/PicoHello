using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.XR.PXR;

namespace HelloPico2.InteractableObjects
{
    public class ArmorTransformSequence : MonoBehaviour
    {
        public Transform _ArmorUpgrade;
        public float _ArmorUpgradeScaleTo = 0;
        public float _ArmorUpgradeScaleDuration;
        public Transform _Armor;
        public float _ArmorScaleTo = 30;
        public float _ArmorScaleDuration;

        public float _ShowGlowDuration;
        public ParticleSystem _Glow;

        private Vector3 ArmorUpgradeScale;

        private void OnEnable()
        {
            _ArmorUpgrade = transform.GetChild(1);

            // Centered Armor
            var pivot = new GameObject("Pivot_" + transform.name); 
            pivot.transform.SetParent(transform);
            pivot.transform.localScale = Vector3.one;
            pivot.transform.position = _ArmorUpgrade.position;
            transform.GetChild(3).SetParent(pivot.transform);
            _Armor = pivot.transform;

            ArmorUpgradeScale = _ArmorUpgrade.transform.localScale; 
            
            _ArmorUpgrade.gameObject.SetActive(true);
            _Armor.gameObject.SetActive(false);
        }
        public void SetUp(Transform armorUpgrade) {
            _ArmorUpgrade = armorUpgrade;

            ArmorUpgradeScale = _ArmorUpgrade.transform.localScale;

            _ArmorUpgrade.gameObject.SetActive(true);
            _Armor.gameObject.SetActive(false);
        }
        public void StartTransform() {
            _ArmorUpgrade.gameObject.SetActive(true);
            _Armor.gameObject.SetActive(true);

            Sequence seq = DOTween.Sequence();

            _ArmorUpgrade.transform.DOScale(_ArmorUpgradeScaleTo, _ArmorUpgradeScaleDuration).From(ArmorUpgradeScale).OnComplete(() => {
                _ArmorUpgrade.gameObject.SetActive(false);
            });
            _Armor.transform.DOScale(_ArmorScaleTo, _ArmorScaleDuration).From(Vector3.zero);

            seq.AppendInterval(_ShowGlowDuration);
            TweenCallback callback = () => { _Glow.Play(true); };
            seq.AppendCallback(callback);

        }
    }
}
