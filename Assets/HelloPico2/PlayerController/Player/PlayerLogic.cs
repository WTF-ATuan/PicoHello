using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project;
using System;
using HelloPico2.InteractableObjects;
using Game.Project;

namespace HelloPico2.PlayerController.Player
{
    public class PlayerLogic : MonoBehaviour
    {
        private PlayerData _PlayerData;

        public PlayerData playerData
        {
            get {
                if (_PlayerData == null)                 
                    _PlayerData = GetComponent<PlayerData>();

                return _PlayerData;
            }
        }
        private ColdDownTimer _InvincibleTimer;
        private void Start()
        {
            _InvincibleTimer = new ColdDownTimer(_PlayerData.invincibleDuration);            
        }
        private void OnEnable()
        {
            playerData.damageDetectionTrigger.TriggerEnter += ReceiveDamage;
        }
        private void OnDisable()
        {
            playerData.damageDetectionTrigger.TriggerEnter -= ReceiveDamage;
        }

        private void ReceiveFeedbacksDamage(Collider other)
        {
            var collide = other.GetComponent<IInteractCollide>();
            collide?.OnCollide(InteractType.Eye, null);

            ReceiveDamageData eventDate = new ReceiveDamageData();
            var hitTarget = other.GetComponentInChildren<HitTargetBase>();
            eventDate.DamageAmount = hitTarget.damageAmount;
            eventDate.InteractType = hitTarget.interactType;

            EventBus.Post(eventDate);

            playerData.armLogic_L.OnEnergyChanged?.Invoke(playerData.armLogic_L.data);
            playerData.armLogic_R.OnEnergyChanged?.Invoke(playerData.armLogic_R.data);
        }
        private void ReceiveDamage(Collider other)
        {
            if (CheckHasShield()) return;
            if (!_InvincibleTimer.CanInvoke()) return;
            if (!CheckInsideHitRadius(other)) return;

            playerData._OnReceiveDamage?.Invoke();

            var collide = other.GetComponent<IInteractCollide>();
            collide?.OnCollide(InteractType.Eye, null);

            ReceiveDamageData eventDate = new ReceiveDamageData();
            var hitTarget = other.GetComponentInChildren<HitTargetBase>();
            eventDate.DamageAmount = hitTarget.damageAmount;
            eventDate.InteractType = hitTarget.interactType;

            EventBus.Post(eventDate);

            playerData.armLogic_L.OnEnergyChanged?.Invoke(playerData.armLogic_L.data);
            playerData.armLogic_R.OnEnergyChanged?.Invoke(playerData.armLogic_R.data);
            
            StartInvincible();
        }
        private bool CheckInsideHitRadius(Collider other) {
            var playerPos = playerData.damageDetectionTrigger.transform.position;
            var targetFlattenPos = other.transform.position;
            targetFlattenPos.y = playerPos.y;
            return Vector3.Distance(playerPos, targetFlattenPos) < playerData.hitRadius;
        }
        private void StartInvincible()
        {
            _InvincibleTimer.Reset();
        }
        private bool CheckHasShield() {
            return _PlayerData.energyBall_L.isCurrentWeaponShield() || _PlayerData.energyBall_R.isCurrentWeaponShield();
        }
    }
}
