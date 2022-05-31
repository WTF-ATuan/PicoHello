using Project;
using UnityEngine;

namespace HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene{
	public class MultiScenePosterExample : MonoBehaviour{
		[SerializeField] private SceneObject sceneKey;

		private float _timer;

		private void OnTriggerEnter(Collider other){
			var multiSceneRequested = new MultiSceneRequested(sceneKey){
				LoadOption = MultiSceneRequested.LoadOptions.BackgroundLoad
			};
			EventBus.Post(multiSceneRequested);
		}

		private void OnTriggerStay(Collider other){
			_timer += Time.deltaTime;
			if(_timer > 3){
				var multiSceneRequested = new MultiSceneRequested(sceneKey){
					LoadOption = MultiSceneRequested.LoadOptions.ActiveBackground
				};
				EventBus.Post(multiSceneRequested);
				_timer = 0;
			}
			
		}

		private void OnTriggerExit(Collider other){
			var multiSceneRequested = new MultiSceneRequested(sceneKey){
				LoadOption = MultiSceneRequested.LoadOptions.UnLoad
			};
			EventBus.Post(multiSceneRequested);
			_timer = 0;
		}
	}
}