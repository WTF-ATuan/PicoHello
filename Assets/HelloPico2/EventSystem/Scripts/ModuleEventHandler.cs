using System;
using System.Collections.Generic;
using System.Linq;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HelloPico2{
	public class ModuleEventHandler : MonoBehaviour{
		[SerializeField] private ViewEventDataOverview overview;

		[Button]
		public void Spawn(string identity, Vector3 position){
			var moduleModel = overview.FindEventData<ModuleModel>(identity);
			var rootModel = moduleModel.modelRoot;
			var activeMesh = moduleModel.activeMesh;
			var clone = GetClone(rootModel, position);
			RemoveUnUseObject(clone, activeMesh.name);
		}

		[Button]
		public void RandomSpawn(Vector3 position){
			var modelDataList = overview.FindAllEvent<ModuleModel>();
			var dataCount = modelDataList.Count;
			var randomIndex = Random.Range(0, dataCount);
			var moduleModel = modelDataList[randomIndex];
			var rootModel = moduleModel.modelRoot;
			var activeMesh = moduleModel.activeMesh;
			var clone = GetClone(rootModel, position);
			RemoveUnUseObject(clone, activeMesh.name);
		}


		private void Start(){
			EventBus.Subscribe<ModuleModelEventRequested>(OnModuleEventRequested);
		}

		private void OnModuleEventRequested(ModuleModelEventRequested obj){
			var modelID = obj.ModelID;
			var position = obj.Position;
			Spawn(modelID, position);
		}

		private GameObject GetClone(GameObject root, Vector3 position){
			var rootClone = Instantiate(root, position, Quaternion.identity);
			rootClone.name = root.name + "(Spawn)";
			return rootClone;
		}

		private void RemoveUnUseObject(GameObject clone, string selectMeshName){
			var childList = clone.GetComponentsInChildren<Transform>(true).ToList();
			var animationRoot = childList.Find(x => x.name.Equals("Char_Guide_Anim"));
			if(!animationRoot) throw new Exception("Can,t find 'Char_Guide_Anim'");
			var foundMeshList = childList.FindAll(x => x.name.Contains("Mesh"));
			foreach(var mesh in from mesh in foundMeshList
					let equalsName = mesh.name.Equals(selectMeshName)
					where !equalsName
					select mesh){
				Destroy(mesh.gameObject);
			}
		}
	}
}