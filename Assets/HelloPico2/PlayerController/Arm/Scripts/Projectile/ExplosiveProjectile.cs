using System.Collections;
using Sirenix.OdinInspector;
using HelloPico2.InteractableObjects;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm
{
    public class ExplosiveProjectile : ProjectileController
    {
        [SerializeField] private float _CountDownDuration = 1;
        [SerializeField] private float _DealDamageRange = 5;
        [SerializeField] private LayerMask _DamageDetectionLayer;
        [SerializeField] private ParticleSystem _ExplosionVFX;
        
        [FoldoutGroup("SFX Settings")][SerializeField] private string _ExplosionSFXName;

        Coroutine timer;

        private void OnEnable()
        {
            timer = StartCoroutine(Timer());
        }
        private IEnumerator Timer() { 
            yield return new WaitForSeconds(_CountDownDuration);
            CheckSurroundedTarget();
            PlayExplosionEffect();
        }
        protected override void OnTriggerEnter(Collider other)
        {
            if (CheckSurroundedTarget())
            {                
                PlayExplosionEffect();
                StopCoroutine(timer);
            }
        }
        private bool CheckSurroundedTarget() {
            var hitInfo = Physics.OverlapSphere(transform.position, _DealDamageRange, _DamageDetectionLayer);

            if (hitInfo.Length > 0)
            {
                for (int i = 0; i < hitInfo.Length; i++)
                {
                    if (hitInfo[i].TryGetComponent<IInteractCollide>(out var interact))
                    {
                        interact.OnCollide(_InteractType, GetComponent<Collider>());
                    }
                }

                return true;
            }
            else
                return false;
        }
        private void PlayExplosionEffect() {
            _ExplosionVFX.gameObject.SetActive(true);
            _ExplosionVFX.Play();
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            _rigidbody.velocity = Vector3.zero;
            
            Project.EventBus.Post(new AudioEventRequested(_ExplosionSFXName, transform.position));
            WhenCollide?.Invoke();

            Destroy(gameObject, _CollideDestryDelayTime);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _DealDamageRange);
        }
    }
}

