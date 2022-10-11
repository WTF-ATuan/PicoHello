using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.PlayerController.Arm;

namespace HelloPico2.ScriptableObjects
{
    [CreateAssetMenu(menuName = "HelloPico2/ScriptableObject/ArmorWeaponData")]
    public class ArmorWeaponsData : ScriptableObject
    {
        [System.Serializable]
        public struct ArmorWeapons
        {
            public ArmorType armorType;
            public GameObject fullEnergyBall;
            public GameObject ChargingEnergyBall;
        }
        public List<ArmorWeapons> armorWeapons = new List<ArmorWeapons>();

        public ArmorWeapons GetData(ArmorType armorType) {
            foreach (var weapon in armorWeapons) { 
                if(weapon.armorType == armorType)
                    return weapon;
            }
            throw new System.Exception("Cannot Find " + armorType);
        }
    }
}
