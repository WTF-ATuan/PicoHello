using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour{
	[SerializeField] public SceneControllerSettingsObject settings;

	public readonly Dictionary<string, AsyncOperation> AsyncOperations = new Dictionary<string, AsyncOperation>();

	public List<Scene> fullyLoadedScenes = new List<Scene>();

	private void Start(){
		Application.backgroundLoadingPriority = ThreadPriority.Low;
		foreach(var sceneName in settings.loadAtStartScenes) LoadScene(sceneName, true);
	}

	public bool Exists(string sceneName){
		return settings.scenes.Contains(sceneName, StringComparer.OrdinalIgnoreCase);
	}

	public void LoadScene(string sceneName){
		StartCoroutine(LoadingScene(sceneName, true));
	}

	public void LoadScene(string sceneName, bool allowSceneActivation){
		StartCoroutine(LoadingScene(sceneName, allowSceneActivation));
	}

	private IEnumerator LoadingScene(string sceneName, bool autoActivation = false){
		for(var i = 0; i < SceneManager.sceneCount; i++)
			if(sceneName == SceneManager.GetSceneAt(i).name) // If the scene we are trying to load is already open stop
				yield break;

		var async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		async.allowSceneActivation = autoActivation;
		AsyncOperations.Add(sceneName, async);
		yield return async;
		SceneLoaded(sceneName);
	}


	private void SceneLoaded(string sceneName){
		AsyncOperations[sceneName] = null;
		AsyncOperations.Remove(sceneName);
		fullyLoadedScenes.Add(SceneManager.GetSceneByName(sceneName));
	}

	public void UnloadScene(string exception){
		StartCoroutine(UnloadingScene(exception));
	}

	private IEnumerator UnloadingScene(string exception){
		// Wait until the end of the frame
		yield return new WaitForEndOfFrame();
		// Scenes marked to be unloaded
		var scenesToUnload = new List<string>();
		// For all of the currently loaded scenes
		for(var i = 0; i < SceneManager.sceneCount; i++) // Make sure the scene is not a persistent scene
			if(settings.persistentScenes.Contains(SceneManager.GetSceneAt(i).name) ==
			   false) // Make sure this scene is not in our list of exceptions
				if(exception != SceneManager.GetSceneAt(i).name ==
				   false) // Add this scene's name to a list of scenes to be unloaded (or index 'i' will change during this for loop)
					scenesToUnload.Add(SceneManager.GetSceneAt(i).name);

		// Now that we're done iterating through each of the Scenes, we can safely unload all the levels
		if(scenesToUnload.Count != 0)
			foreach(var sceneName in scenesToUnload){
				// Remove the scene from 'Fully Loaded Scenes' list
				fullyLoadedScenes.Remove(SceneManager.GetSceneByName(sceneName));
				// Tell SceneManager to unload the scene
				SceneManager.UnloadScene(sceneName);
			}
	}
}