using System;
using System.Collections.Generic;
using AV.Inspector.Runtime;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace HelloPico2{
	[Serializable]
	public class ModuleModel : ViewEventData{
		[Required] [OnValueChanged("OnModelRootChange")] [PreviewField]
		public GameObject modelRoot;

		public List<ChildWrapper> modelComponent = new List<ChildWrapper>();

		#if UNITY_EDITOR
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
				var childGameObject = child.gameObject;
				var childWrapper = new ChildWrapper{
					value = childGameObject,
					parent = child.parent,
					active = childGameObject.activeSelf
				};
				modelComponent.Add(childWrapper);
				GetChildRecursive(child.gameObject);
			}
		}

		#endif
	}

	[Serializable]
	public class ChildWrapper{
		[HideLabel] [HorizontalGroup("1", LabelWidth = 20f, Width = 0.1f)]
		public bool active;

		[GUIColor("GetButtonColor")] [HorizontalGroup("1", LabelWidth = 40f)]
		public GameObject value;

		[HideInInspector] public Transform parent;

		private Color GetButtonColor(){
			return active ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1);
		}
	}
}