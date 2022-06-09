using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Project;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBase : MonoBehaviour, IInteractCollide{

        [FoldoutGroup("Audio Settings")][SerializeField] private string[] _CollideClipName;
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
