using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[CreateAssetMenu(fileName = "SceneControllerSettings", menuName = "SceneControllerSettings", order = 1)]
[System.Serializable]
public class SceneControllerSettingsObject : ScriptableObject {
    // Every scene in the project
    public List<string> scenes = new List<string>();
    // Scenes which will be loaded at start
    public List<string> loadAtStartScenes = new List<string>();
    // List of scenes which will NOT be unloaded when UnloadScenes() is called
    public List<string> persistentScenes = new List<string>();
    
}