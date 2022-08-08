using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm{
	public class ArmorController : MonoBehaviour{
		[ChildGameObjectsOnly] public List<GameObject> armorList;

		private List<GameObject> _armorParts = new List<GameObject>();

		public UltEvents.UltEvent WhenGainAutoActivateArmor;
		public delegate void ArmorDel(GameObject part);
		public ArmorDel WhenActivateArmor;
		private List<ValueDropdownItem> GetChildArmor(){
			var dropdownItems = armorList.Select(x => new ValueDropdownItem(x.name, x));
			return dropdownItems.ToList();
		}

		[Button]
		private void CloseChildActive(bool active){
			foreach(var armor in armorList){
				var componentsInChildren = armor.GetComponentsInChildren<Transform>().ToList();
				componentsInChildren.RemoveAt(0);
				componentsInChildren.ForEach(x => x.gameObject.SetActive(active));
			}
		}

		[Button]
		public void AutoActiveWithOrder(ArmorType type){
			var armorParts = Enum.GetValues(typeof(ArmorPart)).Cast<ArmorPart>().ToList();
			var armorPartNames = armorParts.Select(x => x.ToString()).ToList();
			var lastIndex = 0;
			for(var i = 0; i < armorPartNames.Count; i++){
				var partName = armorPartNames[i];
				var exists = _armorParts.Exists(x => x.name.Contains(partName));
				if(!exists) continue;
				if(i + 1 > lastIndex){
					lastIndex = i + 1;
				}
			}

			if(lastIndex > armorParts.Count){
				return;
			}

			var armorPart = armorParts[lastIndex];
			ActiveArm(type, armorPart);

			WhenGainAutoActivateArmor?.Invoke();			
		}

		[Button]
		public void ActiveArm(ArmorType type, ArmorPart part){
			var existsPart = _armorParts.Find(x => x.name.Contains(part.ToString()));
			if(existsPart){
				existsPart.SetActive(false);
				_armorParts.Remove(existsPart);
			}

			var foundArmor = armorList.Find(x => x.name.Contains(type.ToString()));
			if(!foundArmor) throw new Exception($"Can,t find {type} of GameObject");
			var partsList = foundArmor.GetComponentsInChildren<Transform>(true).ToList();
			var foundPart = partsList.Find(x => x.name.Contains(part.ToString()));
			if(!foundPart) throw new Exception($"can,t find {part} of {foundArmor}");
			_armorParts.Add(foundPart.gameObject);
			foundPart.gameObject.SetActive(true);

			WhenActivateArmor?.Invoke(foundPart.gameObject);
		}

		[Button]
		public List<GameObject> GetActiveArmParts(){
			return _armorParts;
		}
	}

	public enum ArmorType{
		Diligence,
		Emotion,
		Integrity,
		Knowledge,
		Mercy,

		Nature //需要流體手
		//Armor 不需要流體手
	}

	public enum ArmorPart{
		Fingers = 0,
		HandController = 1,
		Elbow = 2,
		Forearm = 3,
		UpperArm = 4
	}
}