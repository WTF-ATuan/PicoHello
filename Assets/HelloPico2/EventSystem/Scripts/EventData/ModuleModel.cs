using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2{
	[Serializable]
	public class ModuleModel : ViewEventData{
		[LabelWidth(200)]
		[Required]
		[OnValueChanged("OnModelRootChange")]
		[PreviewField(150, ObjectFieldAlignment.Left)]
		public GameObject modelRoot;

		public List<ChildWrapper> modelComponent = new List<ChildWrapper>();
		private List<ChildWrapper> _sceneObjectModelBuffer = new List<ChildWrapper>();

		#if UNITY_EDITOR
		private void OnModelRootChange(){
			if(!modelRoot){
				modelComponent.Clear();
				return;
			}

			modelComponent.Clear();
			_sceneObjectModelBuffer.Clear();
			var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(modelRoot);
			var assetGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
			if(assetGameObject == null) throw new Exception("this is not prefab");
			GetChildRecursive(modelRoot);
			ComparePrefabChild(assetGameObject);
			modelRoot = assetGameObject;
		}

		private void GetChildRecursive(GameObject obj){
			if(null == obj)
				return;
			foreach(Transform child in obj.transform){
				if(null == child)
					continue;
				var childGameObject = child.gameObject;
				var childWrapper = new ChildWrapper{
					value = childGameObject,
					parent = child.parent,
					active = childGameObject.activeSelf
				};
				_sceneObjectModelBuffer.Add(childWrapper);
				GetChildRecursive(childGameObject);
			}
		}

		private void ComparePrefabChild(GameObject pre){
			if(null == pre)
				return;
			foreach(Transform child in pre.transform){
				if(!child) continue;
				var childGameObject = child.gameObject;
				var findWrapper = _sceneObjectModelBuffer.Find(x => x.value.name.Equals(childGameObject.name));
				var isActive = findWrapper.active && findWrapper.parent.gameObject.activeSelf;
				if(!isActive) continue;
				var childWrapper = new ChildWrapper{
					value = childGameObject,
					parent = child.parent,
					active = true
				};
				modelComponent.Add(childWrapper);
				ComparePrefabChild(childGameObject);
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