using Project;
using UnityEngine;

namespace HelloPico2{
	public class ModulePosterExample : MonoBehaviour{
		[SerializeField] private Transform spawnPosition;

		public void Spawn(string modelID){
			var moduleModelEventRequested = new ModuleModelEventRequested(modelID, spawnPosition.position);
			EventBus.Post(moduleModelEventRequested);
		}
	}
}