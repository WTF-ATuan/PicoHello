using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PlayerController.BeamCharge
{
    public class LaserDamageDetector : MonoBehaviour
    {
        public PlayerTwoHandBehavior _PlayerTwoHandBehavior;
        [SerializeField] private float _SphereCastRadius = 1;
        [SerializeField] private float _CastDistance = 500;
        [SerializeField] private LayerMask _CastLayer;

        [Header("Deal Damage Settings")]
        [SerializeField] private float _Period;

        private Ray ray;
        [ReadOnly][SerializeField] private RaycastHit[] hitInfos = new RaycastHit[20];

        private Coroutine LaserShootProcess;

        public void StartLaserShootSequence() {
            if (LaserShootProcess == null)
                LaserShootProcess = StartCoroutine(DoLaserShoot());
        }
        public void EndLaserShootSequence()
        {
            if (LaserShootProcess != null)
                StopCoroutine(LaserShootProcess);
        }
        public void LaserShoot() {
            ray = new Ray();
            ray.origin = _PlayerTwoHandBehavior._BeamChargeController.transform.position;
            ray.direction = _PlayerTwoHandBehavior._BeamChargeController.transform.forward;

            if (Physics.SphereCastNonAlloc(ray, _SphereCastRadius, hitInfos, _CastDistance, _CastLayer) != 0)
            {
                print("Found Target");
                DealDamage(); 
            }
        }
        private IEnumerator DoLaserShoot() {
            while (true) {
                LaserShoot();
                yield return new WaitForSeconds(_Period);
            }
        }
        private void DealDamage() {
            for (int i = 0; i < hitInfos.Length; i++)
            {
                if (hitInfos[i].collider == null) continue;
                if (hitInfos[i].collider.TryGetComponent<IInteractCollide>(out var interactCollide)) {
                    interactCollide.OnCollide(InteractType.Energy, hitInfos[i].collider);
                }                
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var from = transform.position;
            Gizmos.DrawWireSphere(from, _SphereCastRadius);
            from = transform.position + transform.up * _SphereCastRadius; 
            Gizmos.DrawLine(from, from + transform.forward * _CastDistance);

            from = transform.position - transform.up * _SphereCastRadius; 
            Gizmos.DrawLine(from, from + transform.forward * _CastDistance);

            from = transform.position - transform.right * _SphereCastRadius; 
            Gizmos.DrawLine(from, from + transform.forward * _CastDistance);

            from = transform.position + transform.right * _SphereCastRadius; 
            Gizmos.DrawLine(from, from + transform.forward * _CastDistance);

            Gizmos.DrawWireSphere(transform.position + transform.forward * _CastDistance, _SphereCastRadius);
        }
    }
}
