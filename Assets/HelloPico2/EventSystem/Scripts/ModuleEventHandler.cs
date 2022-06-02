using System;
using System.Collections.Generic;
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
					
			
			
			
		}
	}
}