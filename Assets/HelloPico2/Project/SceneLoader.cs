using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project{
	public class SceneLoader : MonoBehaviour{
		private readonly List<AsyncOperation> _sceneToLoad = new List<AsyncOperation>();

		public void AddScene(string sceneName){
			var scene = SceneManager.GetSceneByName(sceneName);
			if(scene == default) throw new Exception($"Can,t find {sceneName} check build setting");
			var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName);
			_sceneToLoad.Add(loadSceneAsync);
		}

		public void RemoveScene(string sceneName){
			var scene = SceneManager.GetSceneByName(sceneName);
			if(scene == default) throw new Exception($"Can,t find {sceneName} check build setting");
			var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName);
			_sceneToLoad.Remove(loadSceneAsync);
		}
	}
}