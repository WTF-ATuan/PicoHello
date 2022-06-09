using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;

namespace HelloPico2.PlayerController.Arm
{
    public class ArmArmorController : MonoBehaviour
    {
        [System.Serializable]
        public struct ArmorActivationSettings
        {
            [PropertyRange(0,1)] public float healthPercentage;
            public GameObject armor;
        }
        public List<ArmorActivationSettings> _ArmorActivationSettings = new List<ArmorActivationSettings>();
        [SerializeField] private ArmLogic _ArmLogic;
        //private ArmLogic armLogic
        //{
        //    get { 
        //        if(_ArmLogic == null) 
        //            _ArmLogic = GetComponent<ArmLogic>();

        //        return _ArmLogic;
        //    }
        //}

        private void OnEnable()
        {
            _ArmLogic.OnEnergyChanged += UpdateArmor;
        }
        private void UpdateArmor(ArmData data) {
            var percentage = data.Energy / data.MaxEnergy;

            for (int i = 0; i < _ArmorActivationSettings.Count; i++)
            {
                if(percentage < _ArmorActivationSettings[i].healthPercentage)
                    _ArmorActivationSettings[i].armor.SetActive(false);
                if(percentage >= _ArmorActivationSettings[i].healthPercentage)
                    _ArmorActivationSettings[i].armor.SetActive(true);
            }
        }
    }
}
