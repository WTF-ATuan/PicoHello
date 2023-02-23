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
        public Follower _FollowMainCam;
        public Transform _Energy;
        public float _EnergyZOffset = -0.85f;
        public float _EnergyMovingDuration = 0.3f;
        public delegate void PickableEnergyDel(PickableEnergy energy, InteractCollider handCol);
        public PickableEnergyDel OnPlayerGetEnergy;        
        [ReadOnly][SerializeField] private State _CurrentState = State.ReadyToBePick;
        private bool _AutoGrab = false;

        private void Awake()
        {
            _FollowMainCam.enabled = false;
        }
        private void Start()
        {
            _FollowMainCam.Target = HelloPico2.Singleton.GameManagerHelloPico.Instance._MainCamera.transform;
            _FollowMainCam.enabled = true;
        }

        public override void OnTriggerEnter(Collider other)
        {
            //if (_AutoGrab) return;
            if (_CurrentState != State.ReadyToBePick) return;

            base.OnTriggerEnter(other);

            if (other.TryGetComponent<InteractCollider>(out var interactCollider))
                FollowingHand(interactCollider);
        }
        public void DisableEnergyFollower() { 
            _FollowMainCam.enabled = false;
        }
        public void UseAutoGrab() {
            _AutoGrab = true;
        }
        public void FollowingHand(InteractCollider interactCollider) {
            //print(transform.name + " " + _HandType + " " + interactCollider.name + " " + interactCollider._HandType);
            if (interactCollider._HandType != _HandType) return;
            _Energy.DOLocalMoveZ(_EnergyZOffset, _EnergyMovingDuration).SetEase(Ease.InOutCubic);
            _Follower.Target = interactCollider.transform;
            OnPlayerGetEnergy(this, interactCollider);
            
            _CurrentState = State.Picked;

            _FollowMainCam.enabled = false;
        }
    }
}
