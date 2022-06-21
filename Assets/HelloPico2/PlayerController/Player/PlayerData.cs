using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.PlayerController.Arm;
using MoreMountains.Feel;

namespace HelloPico2.PlayerController.Player
{
    public class PlayerData : MonoBehaviour
    {
        [SerializeField] private ArmLogic _ArmLogic_L;
        [SerializeField] private ArmLogic _ArmLogic_R;
        [SerializeField] private TriggerBase _DamageDetectionTrigger;
        [SerializeField] private MoreMountains.Feedbacks.MMFeedbacks _ReceiveDamageFeedback;

        public ArmLogic armLogic_L { get { return _ArmLogic_L; } }
        public ArmLogic armLogic_R { get { return _ArmLogic_R; } }
        public TriggerBase damageDetectionTrigger { get { return _DamageDetectionTrigger; } }
        public MoreMountains.Feedbacks.MMFeedbacks receiveDamageFeedback { get { return _ReceiveDamageFeedback; } }
    }
}