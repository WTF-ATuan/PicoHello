using HelloPico2.InputDevice.Scripts;
using HelloPico2.PlayerController.Arm;
using DG.Tweening;
using UnityEngine;
using AV.Inspector.Runtime;

namespace HelloPico2.PlayerController.BeamCharge
{
    public class PickableEnergy : TriggerBase
    {
        public enum State { ReadyToBePick, Picked, Charging}
        public HandType _HandType;
        public Follower _Follower;
        public Transform _Energy;
        public float _EnergyZOffset = -0.85f;
        public float _EnergyMovingDuration = 0.3f;
        public delegate void PickableEnergyDel(PickableEnergy energy, InteractCollider handCol);
        public PickableEnergyDel OnPlayerGetEnergy;        
        [ReadOnly][SerializeField] private State _CurrentState = State.ReadyToBePick;

        public override void OnTriggerEnter(Collider other)
        {
            if(_CurrentState != State.ReadyToBePick) return;

            base.OnTriggerEnter(other);

            if (other.TryGetComponent<InteractCollider>(out var interactCollider))
                FollowingHand(interactCollider);
        }
        private void FollowingHand(InteractCollider interactCollider) {
            if (interactCollider._HandType != _HandType) return;
            _Energy.DOLocalMoveZ(_EnergyZOffset, _EnergyMovingDuration).SetEase(Ease.InOutCubic);
            _Follower.Target = interactCollider.transform;
            OnPlayerGetEnergy(this, interactCollider);
            
            _CurrentState = State.Picked;
        }
    }
}
