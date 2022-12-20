using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace HelloPico2.LevelTool{
	public class SelectArmExample : MonoBehaviour{
		public List<GameObject> selectedObjects = new List<GameObject>();
		public TargetItem_SO itemSo;

		private void Start(){
			//全部物件關閉
			selectedObjects.ForEach(x => x.SetActive(false));
		}

		[Button]
		public void OpenSelectItem(){
			var targetItemName = itemSo.targetItemName;
			var foundObject = selectedObjects.Find(x => targetItemName.Contains(x.name));
			if(foundObject) foundObject.SetActive(true);
		}
	}
}