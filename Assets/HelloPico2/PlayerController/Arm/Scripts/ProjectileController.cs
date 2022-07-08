using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private InteractType _InteractType = InteractType.EnergyBall;
        [SerializeField] private float _Lifetime = 10;
        [SerializeField] private bool _ActivateHoming = false;
        [ShowIf("_ActivateHoming")][SerializeField] private float _homingSensativeness;
        [ShowIf("_ActivateHoming")][SerializeField] private float _homingDuration;

        public UnityEngine.Events.UnityEvent WhenShoot;

        private Rigidbody _rigidbody;
        private float _speed;
        private float _duration;
        private AnimationCurve _easingCurve;
        float _step;
        Transform _target;
        Vector3 dir;
        public Vector3 velocity;
        public bool finishedVelocityBuffer;
        public void ProjectileSetUp(float speed, float duration, AnimationCurve easingCurve, Transform target = null) {
            if (target) _target = target;
            _rigidbody = GetComponent<Rigidbody>();
            _speed = speed;
            _duration = duration;
            _easingCurve = easingCurve;
            Destroy(gameObject, _Lifetime);
            WhenShoot?.Invoke();
        }
        private void Update()
        {
            if (!finishedVelocityBuffer)
            {
                _step += Time.deltaTime;

                if (_step / _duration <= 1)
                {
                    _rigidbody.velocity = Vector3.Lerp(Vector3.zero, transform.forward * _speed, _easingCurve.Evaluate(_step / _duration));
                    velocity = _rigidbody.velocity;
                }
                else
                {
                    _rigidbody.velocity = transform.forward * _speed;
                    finishedVelocityBuffer = true;
                }
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
                interact.OnCollide(_InteractType, null);
                Destroy(gameObject);
            }
        }
    }
}
