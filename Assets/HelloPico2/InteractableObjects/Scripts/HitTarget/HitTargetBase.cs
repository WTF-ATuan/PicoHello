using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBase : MonoBehaviour, IInteractCollide{
        public virtual void OnCollide(InteractType type){

        }
    }
}
