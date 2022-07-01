using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project;
using System;
using HelloPico2.InteractableObjects;

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

        private void OnEnable()
        {
            playerData.damageDetectionTrigger.TriggerEnter += ReceiveDamage;
        }
        private void OnDisable()
        {
            playerData.damageDetectionTrigger.TriggerEnter -= ReceiveDamage;
        }

        private void ReceiveDamage(Collider other)
        {
            playerData.receiveDamageFeedback.PlayFeedbacks();

            var collide = other.GetComponent<IInteractCollide>();
            collide?.OnCollide(InteractType.Eye, null);

            ReceiveDamageData eventDate = new ReceiveDamageData();
            var hitTarget = other.GetComponentInChildren<HitTargetBase>();
            eventDate.DamageAmount = hitTarget.damageAmount;
            eventDate.InteractType = hitTarget.interactType;

            EventBus.Post(eventDate);
        }
    }
}
