using System;
using System.Collections.Generic;
using System.Linq;
using Project;
using UnityEngine;

namespace HelloPico2{
	public class ModuleEventHandler : MonoBehaviour{
		[SerializeField] private ViewEventDataOverview overview;

		private void Start(){
			EventBus.Subscribe<ModuleModelEventRequested>(OnModuleEventRequested);
		}

		private void OnModuleEventRequested(ModuleModelEventRequested obj){
			var modelID = obj.ModelID;
			var moduleModel = overview.FindEventData<ModuleModel>(modelID);
			var position = obj.Position;
			var modelRoot = moduleModel.modelRoot;
			var childWrappers = moduleModel.modelComponent;
			SpawnModel(modelRoot, childWrappers, position);
		}

		private void SpawnModel(GameObject root, List<ChildWrapper> childList, Vector3 position){
			var rootClone = Instantiate(root, position, Quaternion.identity);
			RemoveUnMatchChild(rootClone, childList);
		}

		private void RemoveUnMatchChild(GameObject pre, List<ChildWrapper> childList){
			if(null == pre)
				return;
			var referenceNameList = childList.Select(x => x.value.name).ToList();
			foreach(Transform child in pre.transform){
				var childGameObject = child.gameObject;
				var contains = referenceNameList.Contains(childGameObject.name);
				if(!contains){
					Destroy(childGameObject);
				}
				RemoveUnMatchChild(childGameObject, childList);
			}
		}
	}
}