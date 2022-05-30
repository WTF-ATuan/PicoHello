using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Make a static reference to this gamecontroller
    public static SceneController control;

    // Settings file
    [SerializeField]
    public SceneControllerSettingsObject settings;
    
    // The AsyncOperation for loading a level
    public Dictionary<string, AsyncOperation> asyncOperations = new Dictionary<string, AsyncOperation>();

    // All scenes which are currently loaded
    public List<Scene> fullyLoadedScenes = new List<Scene>();

    // Awake and ensure this gameobject is persisent and the only gamecontroller
    void Awake()
    {        
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        //Application.targetFrameRate = 60;
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        foreach (var sceneName in settings.loadAtStartScenes)
        {
            LoadLevel(sceneName, true);
        }

        // Just some test code to show loading multiple scenes simultaneously
        //LoadLevel("RoomB", false);
        //LoadLevel("RoomC", true);
        //LoadLevel("RoomD", false);
        //LoadLevel("RoomE", true);        
    }

    void Update() {
        /*
        foreach (KeyValuePair<string, AsyncOperation> i in asyncOperations) {
            Debug.Log(i.Key + " progress: " + i.Value.progress);
        }*/
    }

    //****************************************************************
    // SceneExists: Check if a scene exists in project
    //****************************************************************
    public bool SceneExists(string name) {
        return settings.scenes.Contains(name, StringComparer.OrdinalIgnoreCase);
    }

    //****************************************************************
    // Load Level (string sceneName)
    //****************************************************************
    public void LoadLevel(string sceneName) {
        StartCoroutine(ILoadLevel(sceneName));
    }
    private IEnumerator ILoadLevel(string sceneName) {
        // Wait until the end of the current frame
        yield return new WaitForEndOfFrame();
        // Look at all currently opened scenes
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            // If the scene we are trying to load is already open
            if (sceneName == SceneManager.GetSceneAt(i).name) {
                // Stop now
                yield break;
            }
        }
        // Load the new scene additively
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        // Set the async allowSceneActivation bool to bool passed into this function
        async.allowSceneActivation = true;
        // Add this async operation to the list of current operations (manages loading multiple levels simultaneously)
        asyncOperations.Add(sceneName, async);
        // Yield the async operation;
        yield return async;
        // Scene is now fully loaded
        StartCoroutine(SetLoadedScene(sceneName));
    }
    //****************************************************************
    // Load Level (string sceneName, bool allowSceneActivation)
    //****************************************************************
    public void LoadLevel(string sceneName, bool allowSceneActivation) {
        StartCoroutine(ILoadLevel(sceneName, allowSceneActivation));
    }
    private IEnumerator ILoadLevel(string sceneName, bool allowSceneActivation) {
        // Wait until the end of the current frame
        yield return new WaitForEndOfFrame();
        // Look at all currently opened scenes
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            // If the scene we are trying to load is already open
            if (sceneName == SceneManager.GetSceneAt(i).name) {
                // Stop now
                yield break;
            }
        }
        // Load the new scene additively
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        // Set the async allowSceneActivation bool to bool passed into this function
        async.allowSceneActivation = allowSceneActivation;
        // Add this async operation to the list of current operations (manages loading multiple levels simultaneously)
        asyncOperations.Add(sceneName, async);
        // Yield the async operation;
        yield return async;
        // Scene is now fully loaded
        StartCoroutine(SetLoadedScene(sceneName));
    }

    //****************************************************************
    // Set Loaded Scene (remove async operation, move scene to Loaded list)
    //****************************************************************
    private IEnumerator SetLoadedScene(string sceneName) {
        yield return new WaitForEndOfFrame();
        asyncOperations[sceneName] = null;
        asyncOperations.Remove(sceneName);
        fullyLoadedScenes.Add(SceneManager.GetSceneByName(sceneName));
    }

    //****************************************************************
    // Unload Levels() Persistent Scenes will not be unloaded
    //****************************************************************
    public void UnloadLevels() {
        StartCoroutine(IUnloadLevels());
    }

    private IEnumerator IUnloadLevels() {
        // Wait until the end of the frame
        yield return new WaitForEndOfFrame();
        // Scenes marked to be unloaded
        List<string> scenesToUnload = new List<string>();
        // For all of the currently loaded scenes
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            // Make sure the scene is not a persistent scene
            if (settings.persistentScenes.Contains(SceneManager.GetSceneAt(i).name) == false) {
                // Add this scene's name to a list of scenes to be unloaded (or index 'i' will change during this for loop)
                scenesToUnload.Add((string)SceneManager.GetSceneAt(i).name);
            }
        }
        // Now that we're done iterating through each of the Scenes, we can safely unload all the levels
        if (scenesToUnload.Count != 0) {
            foreach (string sceneName in scenesToUnload) {
                // Remove the scene from 'Fully Loaded Scenes' list
                fullyLoadedScenes.Remove(SceneManager.GetSceneByName(sceneName));
                // Tell SceneManager to unload the scene
                SceneManager.UnloadScene(sceneName);
            }
        }
    }
    //****************************************************************
    // Unload Levels (string excpetion) Exceptions will not be unloaded
    //****************************************************************
    public void UnloadLevels(string exception) {
        StartCoroutine(IUnloadLevels(exception));
    }
    private IEnumerator IUnloadLevels(string exception) {
        // Wait until the end of the frame
        yield return new WaitForEndOfFrame();
        // Scenes marked to be unloaded
        List<string> scenesToUnload = new List<string>();
        // For all of the currently loaded scenes
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            // Make sure the scene is not a persistent scene
            if (settings.persistentScenes.Contains(SceneManager.GetSceneAt(i).name) == false) {
                // Make sure this scene is not in our list of exceptions
                if (exception != SceneManager.GetSceneAt(i).name == false) {
                    // Add this scene's name to a list of scenes to be unloaded (or index 'i' will change during this for loop)
                    scenesToUnload.Add((string)SceneManager.GetSceneAt(i).name);
                }
            }
        }
        // Now that we're done iterating through each of the Scenes, we can safely unload all the levels
        if (scenesToUnload.Count != 0) {
            foreach (string sceneName in scenesToUnload) {
                // Remove the scene from 'Fully Loaded Scenes' list
                fullyLoadedScenes.Remove(SceneManager.GetSceneByName(sceneName));
                // Tell SceneManager to unload the scene
                SceneManager.UnloadScene(sceneName);
            }
        }
    }
    //****************************************************************
    // Unload Levels (string[] excpetions) Exceptions will not be unloaded
    //****************************************************************
    public void UnloadLevels(string[] exceptions) {
        StartCoroutine(IUnloadLevels(exceptions));
    }
    private IEnumerator IUnloadLevels(string[] exceptions)
    {
        // Wait until the end of the frame
        yield return new WaitForEndOfFrame();
        // Scenes marked to be unloaded
        List<string> scenesToUnload = new List<string>();
        // For all of the currently loaded scenes
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            // Make sure the scene is not a persistent scene
            if (settings.persistentScenes.Contains(SceneManager.GetSceneAt(i).name) == false)
            {
                // Make sure this scene is not in our list of exceptions
                if (exceptions.Contains(SceneManager.GetSceneAt(i).name) == false)
                {
                    // Add this scene's name to a list of scenes to be unloaded (or index 'i' will change during this for loop)
                    scenesToUnload.Add((string)SceneManager.GetSceneAt(i).name);
                }
            }
        }
        // Now that we're done iterating through each of the Scenes, we can safely unload all the levels
        if (scenesToUnload.Count != 0)
        {
            foreach (string sceneName in scenesToUnload)
            {
                // Remove the scene from 'Fully Loaded Scenes' list
                fullyLoadedScenes.Remove(SceneManager.GetSceneByName(sceneName));
                // Tell SceneManager to unload the scene
                SceneManager.UnloadScene(sceneName);                
            }
        }
    }
}