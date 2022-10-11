using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.ScriptableObjects;
using Project;
using HelloPico2.InteractableObjects;

namespace HelloPico2.PlayerController.Arm
{
    public class ArmorWeaponsSettings : MonoBehaviour
    {
        public ArmorWeaponsData _ArmorWeaponsSetting;
        EnergyBallBehavior energyball;
        private void OnEnable()
        {
            EventBus.Subscribe<GainArmorUpgradeData>(ArmorUpgrade);
            energyball = GetComponent<EnergyBallBehavior>();
        }
        private void ArmorUpgrade(GainArmorUpgradeData data)
        {
            ArmorWeaponsData.ArmorWeapons armorWeaponsData = _ArmorWeaponsSetting.GetData(data.armorType);

            energyball.ChargedEnergyProjectile = armorWeaponsData.fullEnergyBall;
            energyball.ChangeChargingEnergyBall(armorWeaponsData.ChargingEnergyBall);
        }
    }
}
