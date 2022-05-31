using UnityEngine.Events;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetFlower : HitTargetBase
    {
        public UnityEvent WhenCollide;

        public override void OnCollide(InteractType type)
        {
            base.OnCollide(type);
            WhenCollide?.Invoke();
        }
    }
}
