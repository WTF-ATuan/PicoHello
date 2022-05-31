using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project{
	public class SceneLoader : MonoBehaviour{
		private readonly Dictionary<string, AsyncOperation> _sceneToLoad = new Dictionary<string, AsyncOperation>();

		public void LoadScene(string sceneName){
			var containsKey = _sceneToLoad.ContainsKey(sceneName);
			if(!containsKey) throw new Exception($" {sceneName} is Loading");
			var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			loadSceneAsync.allowSceneActivation = false;
			loadSceneAsync.completed += OnSceneAsyncCompleted;
			_sceneToLoad.Add(sceneName, loadSceneAsync);
		}

		private void OnSceneAsyncCompleted(AsyncOperation obj){ }

		public void RemoveScene(){
			
		}
	}
}