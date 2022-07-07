using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace HelloPico2{
	[Serializable]
	public class ModuleModel : ViewEventData{
		[Required] [OnValueChanged("OnModelRootChange")]
		public GameObject modelRoot;

		[ReadOnly] public GameObject animationRoot;

		[ValueDropdown("GetAllMeshObject")] [ShowIf("animationRoot")]
		public GameObject activeMesh;

		private List<GameObject> _modelChildList = new List<GameObject>();

		private void OnModelRootChange(){
			GetChildRecursive(modelRoot);
			animationRoot = _modelChildList.Find(x => x.name.Equals("Char_Guide_Anim"));
		}

		private void GetChildRecursive(GameObject obj){
			if(null == obj)
				return;
			foreach(Transform child in obj.transform){
				if(null == child)
					continue;
				var childGameObject = child.gameObject;
				_modelChildList.Add(childGameObject);
				GetChildRecursive(childGameObject);
			}
		}

		private IEnumerator GetAllMeshObject(){
			if(!animationRoot) return null;
			var foundChild = _modelChildList.FindAll(x => x.name.Contains("Mesh"));
			var valueList = foundChild.Select(x => new ValueDropdownItem(x.name, x)).GetEnumerator();
			return valueList;
		}
	}
}