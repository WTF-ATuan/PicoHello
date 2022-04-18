using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project{
	public class SceneLoader : MonoBehaviour{
		private List<AsyncOperation> _sceneToLoad;

		private void Start(){
			_sceneToLoad = new List<AsyncOperation>();
			var currentScene = SceneManager.GetActiveScene();
			var currentSceneName = currentScene.name;
			var loadSceneAsync = SceneManager.LoadSceneAsync(currentSceneName);
			_sceneToLoad.Add(loadSceneAsync);
		}

		public void AddScene(string sceneName){ }
	}
}