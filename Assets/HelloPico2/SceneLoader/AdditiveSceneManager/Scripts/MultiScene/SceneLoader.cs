using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene{
	[RequireComponent(typeof(SceneController))]
	public class SceneLoader : MonoBehaviour{
		[EnumToggleButtons] public LoadOptions loadOption;

		[HorizontalGroup("Setting")] [HideLabel]
		public SceneObject scene;

		[HorizontalGroup("Setting")] public float delayTime;


		private SceneController _sceneController;

		private void Start(){
			_sceneController = GetComponent<SceneController>();
		}

		public void EventHandle(CrossEvent loadedEvent){
		}

		[Button]
		public async void Invoke(){
			if(delayTime != 0){
				var delaySecond = Mathf.RoundToInt(delayTime * 1000);
				var delayTask = Task.Delay(delaySecond);
				await delayTask;
			}

			switch(loadOption){
				case LoadOptions.Load:
					_sceneController.LoadScene(scene);
					break;
				case LoadOptions.BackgroundLoad:
					_sceneController.BackGroundLoadScene(scene);
					break;
				case LoadOptions.ActiveBackground:
					_sceneController.ActiveBackgroundScene(scene);
					break;
				case LoadOptions.UnLoad:
					_sceneController.UnloadScene(scene);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}