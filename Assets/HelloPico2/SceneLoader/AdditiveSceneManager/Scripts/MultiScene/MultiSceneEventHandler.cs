using System;
using HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.AdditiveSceneManager.Scripts{
	[RequireComponent(typeof(SceneController))]
	public class MultiSceneEventHandler : MonoBehaviour{
		[Required] [SerializeField] private MultiSceneDataOverview sceneDataOverview;
		private SceneController _sceneController;

		private void Start(){
			_sceneController = GetComponent<SceneController>();
			EventBus.Subscribe<MultiSceneRequested>(OnMultiSceneRequested);
		}

		private void OnMultiSceneRequested(MultiSceneRequested obj){
			var sceneObject = obj.SceneObject;
			var loadOption = obj.LoadOption;
			var sceneObjects = sceneDataOverview.FindScene(sceneObject);
			switch(loadOption){
				case MultiSceneRequested.LoadOptions.Load:
					foreach(var scene in sceneObjects) _sceneController.LoadScene(scene);

					break;
				case MultiSceneRequested.LoadOptions.BackgroundLoad:
					foreach(var scene in sceneObjects) _sceneController.BackGroundLoadScene(scene);

					break;
				case MultiSceneRequested.LoadOptions.ActiveBackground:
					foreach(var scene in sceneObjects) _sceneController.ActiveBackgroundScene(scene);

					break;
				case MultiSceneRequested.LoadOptions.UnLoad:
					_sceneController.UnloadScene(sceneObject);

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}