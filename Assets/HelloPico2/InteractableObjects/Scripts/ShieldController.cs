using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UltEvents;

namespace HelloPico2.InteractableObjects
{
    public class ShieldController : MonoBehaviour
    {
        public float _ForceAmount;
        public Collider _Collider;
        public delegate void OnCollisionDel(InteractType interactType, Collider other);
        public OnCollisionDel OnCollision;
        public UltEvent WhenCollision;
              
        private void OnCollisionEnter(Collision collision)
        {
            var collides = collision.gameObject.GetComponents<IInteractCollide>();
            collides.ForEach(c => c?.OnCollide(InteractType.Shield, _Collider));

            foreach (var item in collides)
            {
                if (item != null)
                {
                    print("Collide " + collision.collider.name);
                    var rigidbody = collision.collider.GetComponent<Rigidbody>();
                    rigidbody.isKinematic = false;
                    rigidbody.AddForce(transform.forward * _ForceAmount, ForceMode.Impulse);

                    OnCollision?.Invoke(InteractType.Shield, _Collider);
                    WhenCollision?.Invoke();
                }
            }
        }
    }
}
