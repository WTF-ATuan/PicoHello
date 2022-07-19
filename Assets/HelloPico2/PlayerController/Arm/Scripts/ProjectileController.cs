using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;

namespace HelloPico2.PlayerController.Arm
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private InteractType _InteractType = InteractType.EnergyBall;
        [SerializeField] private float _Lifetime = 10;
        [SerializeField] private float _CollideDestryDelayTime = 3;
        [SerializeField] private bool _ActivateHoming = false;
        [ShowIf("_ActivateHoming")][SerializeField] private float _homingSensativeness;
        [ShowIf("_ActivateHoming")][SerializeField] private float _StartHomingTime;
        [ShowIf("_ActivateHoming")][SerializeField] private float _homingDuration;
        [ShowIf("_ActivateHoming")][SerializeField] private float _AngleLimit = 120;
        [ShowIf("_ActivateHoming")][SerializeField] private float _DistanceLimit = 120;

        public UnityEngine.Events.UnityEvent WhenShoot;

        private Rigidbody _rigidbody;
        private float _speed;
        private float _duration;
        private AnimationCurve _easingCurve;
        float _step;
        Transform _target;
        Vector3 originalDir;
        Vector3 dir;
        Vector3 velocity;
        bool finishedVelocityBuffer;

        public void ProjectileSetUp(float speed, float duration, AnimationCurve easingCurve, Transform target = null, bool homing = false) {
            if (target) _target = target;
            _rigidbody = GetComponent<Rigidbody>();
            _speed = speed;
            _duration = duration;
            _easingCurve = easingCurve;
            _ActivateHoming = homing;
            originalDir = transform.forward;
            Destroy(gameObject, _Lifetime);
            WhenShoot?.Invoke();
        }
        private void Update()
        {
            _step += Time.fixedDeltaTime;

            if (!finishedVelocityBuffer)
            {
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
            else
            {
                if (_ActivateHoming)
                {
                    if (_target == null) return;
                    if (Vector3.Distance(transform.position, _target.position) < 1) { /*_rigidbody.velocity = Vector3.zero;*/ return; }

                    if (_step >= _StartHomingTime && _step <= _homingDuration)
                    {
                        var targetDir = (_target.position - transform.position).normalized;
                                                
                        if (Vector3.Distance(transform.position, _target.position) > _DistanceLimit && 
                            Vector3.Angle(originalDir, targetDir) > _AngleLimit) return;

                        var step = Mathf.Clamp(_step / (1 / _homingSensativeness), 0, 1);

                        dir = Vector3.Lerp(originalDir, targetDir, step);
                        

                        transform.forward = dir;
                        _rigidbody.velocity = dir * _speed;
                    }
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IInteractCollide>(out var interact)) {
                interact.OnCollide(_InteractType, GetComponent<Collider>());
                gameObject.SetActive(false);
                Destroy(gameObject, _CollideDestryDelayTime);
            }
        }
        private void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            GUI.color = Color.green;
            Handles.Label(transform.position + transform.up * _DistanceLimit, "Distance Limit");
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _DistanceLimit);
            
            GUI.color = Color.yellow;
            Handles.Label(transform.position + transform.forward * _DistanceLimit / 2, "Angle Limit");
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * _DistanceLimit);
            Gizmos.DrawRay(transform.position, Quaternion.Euler(_AngleLimit, 0, 0) * transform.forward * _DistanceLimit);
            #endif
        }
    }
}
