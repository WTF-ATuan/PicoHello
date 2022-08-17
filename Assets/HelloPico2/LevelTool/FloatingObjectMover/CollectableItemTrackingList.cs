using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.InteractableObjects
{
    [CreateAssetMenu(menuName = "HelloPico2/ScriptableObject/CollectableItemTracking")]
    public class CollectableItemTrackingList : ScriptableObject
    {
        [System.Serializable]
        public struct TrackingItem {
            public HelloPico2.PlayerController.Arm.ArmorType type;
            public List<InteractableArmorUpgrade> armUpgradeList;
        }
        public List<TrackingItem> _ArmorUpgrades = new List<TrackingItem>();
        [ReadOnly] public List<TrackingItem> trackingItemList = new List<TrackingItem>();

        private InteractableArmorUpgrade GetItem(HelloPico2.PlayerController.Arm.ArmorType type, HelloPico2.PlayerController.Arm.ArmorPart part) {
            int typeIndex = -1;
            int partsIndex = -1;

            for (int i = 0; i < trackingItemList.Count; i++)
            {
                if (trackingItemList[i].type == type)
                {
                    for (int j = 0; j < trackingItemList[i].armUpgradeList.Count; j++)
                    {
                        if (trackingItemList[i].armUpgradeList[j].armorParts == part)
                        {
                            typeIndex = i;
                            partsIndex = j;
                            break;
                        }
                    } 
                }
            }
            if(typeIndex != -1 && partsIndex != -1)
                return trackingItemList[typeIndex].armUpgradeList[partsIndex];
            else
                return null;
        }
    }
}
