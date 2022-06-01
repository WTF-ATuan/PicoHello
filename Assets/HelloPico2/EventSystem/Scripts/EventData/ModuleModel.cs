using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace HelloPico2{
	#if UNITY_EDITOR
	#endif
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

			var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(modelRoot);
			var assetGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
			if(assetGameObject == null) throw new Exception("this is not prefab");
			modelRoot = assetGameObject;
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