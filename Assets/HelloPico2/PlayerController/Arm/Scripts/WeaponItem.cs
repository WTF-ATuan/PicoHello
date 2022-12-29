using System.Collections;
using System.Collections.Generic;
using HelloPico2.InteractableObjects;
using HelloPico2.Interface;
using UnityEngine;


public class WeaponItem : MonoBehaviour, IWeaponFeedbacks
{
    [System.Serializable]
    public struct WeaponForm {
        public InteractableSettings.InteractableType WeaponType;
        public UltEvents.UltEvent WhenSwitchToWeapon; 
    }
    public List<WeaponForm> Weapons = new List<WeaponForm>();
    public InteractableSettings.InteractableType previousType = InteractableSettings.InteractableType.Whip;
    
    protected virtual void OnEnable()
    {
        OnSwithWeapon(previousType);
    }
    public virtual void OnSwithWeapon(InteractableSettings.InteractableType interactableType)
    {
        for (int i = 0; i < Weapons.Count; i++)
        {
            if (interactableType == Weapons[i].WeaponType)
            { 
                Weapons[i].WhenSwitchToWeapon?.Invoke();
            }
        }
        previousType = interactableType;
    }
}
