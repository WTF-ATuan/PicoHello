using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour{
	[SerializeField] public SceneControllerSettingsObject settings;

	public readonly Dictionary<string, AsyncOperation> SceneLoadingList = new Dictionary<string, AsyncOperation>();

	public List<Scene> fullyLoadedScenes = new List<Scene>();

	private void Start(){
		Application.backgroundLoadingPriority = ThreadPriority.Low;
		foreach(var sceneName in settings.loadAtStartScenes) LoadScene(sceneName);
	}

	//如果有效能瓶頸 在來管控 Coroutine;
	public void LoadScene(string sceneName){
		StartCoroutine(LoadingScene(sceneName, true));
	}

	public void UnloadScene(string sceneName){
		StartCoroutine(UnloadingScene(sceneName));
	}

	public void BackGroundLoadScene(string sceneName){
		StartCoroutine(LoadingScene(sceneName));
	}

	public void ActiveBackgroundScene(string sceneName){
		var containsKey = SceneLoadingList.ContainsKey(sceneName);
		if(!containsKey) throw new Exception($"{sceneName} scene is not Loading");
		var asyncOperation = SceneLoadingList[sceneName];
		if(asyncOperation.progress < 0.9f){
			Debug.Log($"{sceneName} loading progress = {asyncOperation.progress}");
		}
		else{
			asyncOperation.allowSceneActivation = true;
		}
	}

	public bool Exists(string sceneName){
		return settings.scenes.Contains(sceneName, StringComparer.OrdinalIgnoreCase);
	}

	private void SceneFullyLoaded(string sceneName){
		SceneLoadingList.Remove(sceneName);
		fullyLoadedScenes.Add(SceneManager.GetSceneByName(sceneName));
	}

	private IEnumerator LoadingScene(string sceneName, bool autoActivation = false){
		for(var i = 0; i < SceneManager.sceneCount; i++)
			if(sceneName == SceneManager.GetSceneAt(i).name) // If the scene we are trying to load is already open stop
				yield break;

		var mode = autoActivation ? LoadSceneMode.Single : LoadSceneMode.Additive;
		var async = SceneManager.LoadSceneAsync(sceneName, mode);
		async.allowSceneActivation = autoActivation;
		SceneLoadingList.Add(sceneName, async);
		yield return async;
		SceneFullyLoaded(sceneName);
	}


	private IEnumerator UnloadingScene(string sceneName){
		var exists = false;
		for(var i = 0; i < SceneManager.sceneCount; i++)
			if(sceneName == SceneManager.GetSceneAt(i).name)
				exists = true;
		if(!exists) yield break;
		var scene = SceneManager.GetSceneByName(sceneName);
		var async = SceneManager.UnloadSceneAsync(scene);
		yield return async;
		fullyLoadedScenes.Remove(scene);
	}
}