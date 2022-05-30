using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public partial class SceneControllerInspector
{
    void RenderPersistentScenes()
    {
        RenderSectionHeader("Persistent Scenes", "These scenes will not be unloaded by the SceneController's UnloadLevels() function.");        
        RenderSceneListPropertyField(persistentScenes);
    }   
}