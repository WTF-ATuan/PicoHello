using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm{
	public class ArmorController : MonoBehaviour{
		[ChildGameObjectsOnly] public List<GameObject> armorList;
		[ValueDropdown("GetChildArmor")] public GameObject currentArmor;
		private List<GameObject> _currentArmorPart;

		[FoldoutGroup("Part Setup")] public bool elbow = true;
		[FoldoutGroup("Part Setup")] public bool fingers = true;
		[FoldoutGroup("Part Setup")] public bool forearm = true;
		[FoldoutGroup("Part Setup")] public bool handController = true;
		[FoldoutGroup("Part Setup")] public bool upperArm = true;

		private List<ValueDropdownItem> GetChildArmor(){
			var dropdownItems = armorList.Select(x => new ValueDropdownItem(x.name, x));
			return dropdownItems.ToList();
		}

		private void Start(){
			if(!currentArmor){
				return;
			}

			armorList.ForEach(x => x.SetActive(false));
			currentArmor.SetActive(true);
			var armors = currentArmor.GetComponentsInChildren<Transform>().ToList();
			_currentArmorPart = armors.Select(x => x.gameObject).ToList();
			UpdateArmPart();
		}
		[Button]
		public void SelectArmor(ArmorType armorType){
			var armorName = armorType.ToString();
			var foundArmor = armorList.Find(x => x.name.Contains(armorName));
			currentArmor = foundArmor;
			armorList.ForEach(x => x.SetActive(false));
			currentArmor.SetActive(true);
			var armors = currentArmor.GetComponentsInChildren<Transform>(true).ToList();
			_currentArmorPart = armors.Select(x => x.gameObject).ToList();
			UpdateArmPart();
		}

		public void UpdateArmPart(){
			foreach(var armorPart in _currentArmorPart){
				var armorPartName = armorPart.name;
				if(armorPartName.Contains("Elbow")){
					armorPart.SetActive(elbow);
				}

				if(armorPartName.Contains("Fingers")){
					armorPart.SetActive(fingers);
				}

				if(armorPartName.Contains("Forearm")){
					armorPart.SetActive(forearm);
				}

				if(armorPartName.Contains("HandController")){
					armorPart.SetActive(handController);
				}

				if(armorPartName.Contains("UpperArm")){
					armorPart.SetActive(upperArm);
				}
			}
		}
	}

	public enum ArmorType{
		Diligence,
		Emotion,
		Integrity,
		Knowledge,
		Mercy,
		Nature,
		Tube
	}
}