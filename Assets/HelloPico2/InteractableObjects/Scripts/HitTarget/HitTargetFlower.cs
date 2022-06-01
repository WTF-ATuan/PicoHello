using UnityEngine.Events;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetFlower : HitTargetBase
    {
        public override void OnCollide(InteractType type)
        {
            base.OnCollide(type);
        }
        private void OnEnable()
        {
            OnEnergyBallInteract += ChargeBloom;            
        }
        private void OnDisable()
        {
            OnEnergyBallInteract -= ChargeBloom;            
        }
        private void ChargeBloom() { 
            WhenCollide?.Invoke();
        }
    }
}
