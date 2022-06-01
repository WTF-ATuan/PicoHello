using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2{
	[Serializable]
	public class ModuleModel : ViewEventData{
		[Required] [OnValueChanged("OnModelRootChange")]
		public GameObject modelRoot;

		public List<GameObject> modelComponent = new List<GameObject>();

		private void OnModelRootChange(){
			if(!modelRoot){
				modelComponent.Clear();
				return;
			}

			GetChildRecursive(modelRoot);
		}

		private void GetChildRecursive(GameObject obj){
			if(null == obj)
				return;

			foreach(Transform child in obj.transform){
				if(null == child || !child.gameObject.activeSelf)
					continue;
				modelComponent.Add(child.gameObject);
				GetChildRecursive(child.gameObject);
			}
		}
	}
}