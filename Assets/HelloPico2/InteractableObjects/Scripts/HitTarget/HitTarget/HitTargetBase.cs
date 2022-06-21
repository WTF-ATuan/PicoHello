using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Project;
using Random = UnityEngine.Random;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBase : MonoBehaviour, IInteractCollide{

        [FoldoutGroup("Audio Settings")][SerializeField] private string[] _CollideClipName;
        [SerializeField] private int _DamageAmount = 0;
        [SerializeField] private InteractType _InteractType = InteractType.Energy;

        public int damageAmount { get { return _DamageAmount; } }
        public InteractType interactType { get { return _InteractType; } }

        public delegate void InteractDel(Collider selfCollider);
        public InteractDel OnBeamInteract;
        public InteractDel OnWhipInteract;
        public InteractDel OnEnergyBallInteract;
        public InteractDel OnShieldInteract;
        public InteractDel OnEnergyInteract;

        public UnityEvent WhenCollide;

        public virtual void OnCollide(InteractType type, Collider selfCollider){
            CheckInteractType(type, selfCollider);
        }

        public Action<InteractType, Collider> ColliderEvent{ get; }

        private void CheckInteractType(InteractType type, Collider selfCollider) {
            switch (type) {
                case InteractType.Beam:
                    OnBeamInteract?.Invoke(selfCollider);
                    break;
                case InteractType.Whip:
                    OnWhipInteract?.Invoke(selfCollider);
                    break;
                case InteractType.EnergyBall:
                    OnEnergyBallInteract?.Invoke(selfCollider);
                    break;
                case InteractType.Shield:
                    OnShieldInteract?.Invoke(selfCollider);
                    break;
                case InteractType.Energy:
                    OnEnergyInteract?.Invoke(selfCollider);
                    break;
                default:
                    WhenCollide?.Invoke();
                    break;
            }
        }
        protected void PlayRandomAudio()
        {
            var value = Random.Range(0, _CollideClipName.Length);

            EventBus.Post(new AudioEventRequested(_CollideClipName[value], transform.position));
        }
    }
}
