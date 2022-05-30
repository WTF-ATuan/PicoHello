using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public partial class SceneControllerInspector
{
    void UnloadScene(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (Application.isPlaying)
        {
            sceneController.fullyLoadedScenes.Remove(scene);
            SceneManager.UnloadScene(sceneName);
        }
        else
        {
            EditorSceneManager.CloseScene(scene, true);
        }
    }

    void LoadScene(string sceneName)
    {        
        if (Application.isPlaying)
        {
            sceneController.LoadLevel(sceneName, true);
        }
        else
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(sceneName + " t:scene")[0]);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        }
    }
}