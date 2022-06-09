using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBell : HitTargetBase
    {
        [SerializeField] private float _Force;
        [SerializeField] private Rigidbody _Rigidbody;
        public GameObject DebugSphere;

        public override void OnCollide(InteractType type, Collider selfCollider)
        {
            base.OnCollide(type, selfCollider);
        }

        private void OnEnable(){
            OnEnergyBallInteract += BellActivate;
            OnBeamInteract += BellActivate;            
        }
        private void OnDisable()
        {
            OnEnergyBallInteract -= BellActivate;
            OnBeamInteract -= BellActivate;            
        }        
       
        private void BellActivate(Collider selfCollider)
        {
            WhenCollide?.Invoke();

            PlayRandomAudio();

            RaycastHit hitInfo = GetContactPoint(selfCollider);

            var dir = (_Rigidbody.transform.position - selfCollider.transform.position).normalized;

            if(hitInfo.point != null)
                dir = (_Rigidbody.transform.position - hitInfo.point).normalized;
                        
            _Rigidbody.AddForce(_Force * dir, ForceMode.VelocityChange);
        }
        public LayerMask _LayerMask;
        private RaycastHit GetContactPoint(Collider selfCollider) {
            RaycastHit hitInfo = new RaycastHit();

            if (selfCollider == null) return hitInfo;

            Ray ray = new Ray(selfCollider.transform.position, selfCollider.transform.forward);

            if (Physics.Raycast(ray, out hitInfo, 1000, _LayerMask)) {
                var clone = Instantiate(DebugSphere, hitInfo.point, Quaternion.identity, transform.parent);
                Destroy(clone, 1.5f);
            }

            return hitInfo;
        }
    }
}