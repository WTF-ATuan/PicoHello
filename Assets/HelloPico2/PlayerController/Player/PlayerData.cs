
using UnityEngine;
using HelloPico2.PlayerController.Arm;
using HelloPico2.InteractableObjects;

namespace HelloPico2.PlayerController.Player
{
    public class PlayerData : MonoBehaviour
    {
        [SerializeField] private ArmLogic _ArmLogic_L;
        [SerializeField] private ArmLogic _ArmLogic_R;
        [SerializeField] private EnergyBallBehavior _EnergyBall_L;
        [SerializeField] private EnergyBallBehavior _EnergyBall_R;
        [SerializeField] private TriggerBase _DamageDetectionTrigger;
        [SerializeField] private EnableEvent _CameraShakeEvent;
        [SerializeField] private ObjectShaker _CameraShakeSettings;
        [SerializeField] private FollowerShaker _LHandShaker;
        [SerializeField] private FollowerShaker _RHandShaker;
        [SerializeField] private float _InvincibleDuration;
        public UltEvents.UltEvent _OnReceiveDamage;

        public ArmLogic armLogic_L { get { return _ArmLogic_L; } }
        public ArmLogic armLogic_R { get { return _ArmLogic_R; } }
        public EnergyBallBehavior energyBall_L { get { return _EnergyBall_L; } }
        public EnergyBallBehavior energyBall_R { get { return _EnergyBall_R; } }
        public TriggerBase damageDetectionTrigger { get { return _DamageDetectionTrigger; } }
        public EnableEvent cameraShakeEvent{get{ return _CameraShakeEvent; }}
        public ObjectShaker cameraShakeSettings { get{ return _CameraShakeSettings; }}
        public FollowerShaker lHandShaker{get{ return _LHandShaker; }}
        public FollowerShaker rHandShaker { get { return _RHandShaker; } }
        public float invincibleDuration { get { return _InvincibleDuration; } private set { _InvincibleDuration = value; } }

        private void Awake()
        {
            var armLogics = GetComponentsInChildren<ArmLogic>();
            _ArmLogic_L = armLogics[0];
            _ArmLogic_R = armLogics[1];
            var energyballs = GetComponentsInChildren<EnergyBallBehavior>();
            _EnergyBall_L = energyballs[0];
            _EnergyBall_R = energyballs[1];
        }
    }
}