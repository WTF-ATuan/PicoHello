using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBase : MonoBehaviour, IInteractCollide{

        [FoldoutGroup("Audio Settings")][SerializeField] private string _CollideClipName;
        public delegate void InteractDel();
        public InteractDel OnBeamInteract;
        public InteractDel OnWhipInteract;
        public InteractDel OnEnergyBallInteract;
        public InteractDel OnShieldInteract;
        public InteractDel OnEnergyInteract;

        public UnityEvent WhenCollide;

        public virtual void OnCollide(InteractType type, Collider selfCollider){
            CheckInteractType(type);
        }
        private void CheckInteractType(InteractType type) {
            switch (type) {
                case InteractType.Beam:
                    OnBeamInteract?.Invoke();
                    break;
                case InteractType.Whip:
                    OnWhipInteract?.Invoke();
                    break;
                case InteractType.EnergyBall:
                    OnEnergyBallInteract?.Invoke();
                    break;
                case InteractType.Shield:
                    OnShieldInteract?.Invoke();
                    break;
                case InteractType.Energy:
                    OnEnergyInteract?.Invoke();
                    break;
                default:
                    WhenCollide?.Invoke();
                    break;
            }
        }
    }
}
