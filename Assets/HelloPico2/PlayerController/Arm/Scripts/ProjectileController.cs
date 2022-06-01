using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using HelloPico2.InteractableObjects;

namespace HelloPico2.PlayerController.Arm
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private InteractType _InteractType = InteractType.EnergyBall;
        private Rigidbody _rigidbody;
        private float _speed;
        private float _duration;
        [SerializeField] private float _Lifetime = 10;
        [SerializeField] private bool _ActivateHoming = false;
        [ShowIf("_ActivateHoming")][SerializeField] private float _homingSensativeness;
        [ShowIf("_ActivateHoming")][SerializeField] private float _homingDuration;


        float _step;
        Transform _target;
        Vector3 dir;

        public void ProjectileSetUp(float speed, float duration, Transform target = null) {
            if (target) _target = target;
            _rigidbody = GetComponent<Rigidbody>();
            _speed = speed;
            _duration = duration;
            Destroy(gameObject, _Lifetime);
        }
        private void Update()
        {
            _step += Time.deltaTime;

            if (_step / _duration <= 1)
            {
                _rigidbody.velocity = Vector3.Lerp(Vector3.zero, transform.forward * _speed, _step / _duration);
            }

            if (!_ActivateHoming)
            {
                if (_target != null && _step <= _homingDuration)
                {
                    dir = (_target.position - transform.position).normalized;
                    dir = Vector3.Lerp(transform.forward, dir, _step / (1 / _homingSensativeness));
                    _rigidbody.velocity = dir * _speed;
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IInteractCollide>(out var interact)) {
                interact.OnCollide(_InteractType);
                Destroy(gameObject);
            }
        }
    }
}
