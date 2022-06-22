using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;
using DG.Tweening;
using UltEvents;

namespace HelloPico2.PlayerController.Arm
{
    public class ArmArmorController : MonoBehaviour
    {
        [System.Serializable]
        public struct ArmorGroup
        {
            public string armorGroupName;
            public List<GameObject> armorUnits;
        }
        [System.Serializable]
        public struct ArmorHealthStatus
        {
            [MinMaxSlider(0,1,true)] public Vector2 healthPercentage;
            public List<ArmorData> armorData;
            public UnityEngine.Events.UnityEvent armorEvent;
        }
        [System.Serializable]
        public struct ArmorData
        {
            public string ControlThisArmorGroup;
            public float speed;
        }
        public List<ArmorGroup> _ArmorGroupSettings = new List<ArmorGroup>();
        public List<ArmorHealthStatus> _ArmorHealthStatus = new List<ArmorHealthStatus>();
        public float _EasingDuration = .5f;

        public UltEvent _EnergyIncreaseEvent;

        [SerializeField] private ArmLogic _ArmLogic;
        //private ArmLogic armLogic
        //{
        //    get { 
        //        if(_ArmLogic == null) 
        //            _ArmLogic = GetComponent<ArmLogic>();

        //        return _ArmLogic;
        //    }
        //}

        private float currentPercentage;

        private void OnEnable()
        {
            _ArmLogic.OnEnergyChanged += UpdateArmor;
            UpdateArmor(_ArmLogic.data);
        }
        [Button]
        private void Test(float percentage) {
            UpdateArmor(percentage);
        }
        private void UpdateArmor(ArmData data) {
            var percentage = data.Energy / data.MaxEnergy;

            if (percentage > currentPercentage) _EnergyIncreaseEvent?.Invoke();

            currentPercentage = percentage;

            UpdateArmor(currentPercentage);
        }
        private void UpdateArmor(float percentage) {
            if (percentage < 0.01f)
            {
                for (int j = 0; j < _ArmorHealthStatus[0].armorData.Count; j++)
                {
                    FindArmorGroup(_ArmorHealthStatus[0].armorData[j].ControlThisArmorGroup, _ArmorHealthStatus[0].armorData[j].speed);
                }
                _ArmorHealthStatus[0].armorEvent?.Invoke();

                return;
            }

            for (int i = 0; i < _ArmorHealthStatus.Count; i++)
            {                
                if (percentage > _ArmorHealthStatus[i].healthPercentage.x && percentage <= _ArmorHealthStatus[i].healthPercentage.y)
                {
                    for (int j = 0; j < _ArmorHealthStatus[i].armorData.Count; j++)
                    {
                        FindArmorGroup(_ArmorHealthStatus[i].armorData[j].ControlThisArmorGroup, _ArmorHealthStatus[i].armorData[j].speed);
                    }

                    _ArmorHealthStatus[i].armorEvent?.Invoke();
                    break;
                }
            }
        }
        private void FindArmorGroup(string groupName, float speed) {
            for (int i = 0; i < _ArmorGroupSettings.Count; i++)
            {
                if (_ArmorGroupSettings[i].armorGroupName == groupName)
                {
                    SetupArmorRotator(_ArmorGroupSettings[i].armorUnits, speed);                    
                }
            }
        }
        private void SetupArmorRotator(List<GameObject> armorUnits, float speed) {
            for (int j = 0; j < armorUnits.Count; j++)
            {
                if (!armorUnits[j].TryGetComponent<ObjectRotator>(out var objRotator)) return;
                
                objRotator.speed = speed;

                DOTween.To(() => objRotator.speed, x => objRotator.speed = x, speed, _EasingDuration);
            }
        }
    }    
}
