using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public partial class SceneControllerInspector
{    
    void RenderLoadAtStartScenes()
    {
        RenderSectionHeader("Scenes to load at Start", "These scenes will be loaded automatically at the start.");
        RenderSceneListPropertyField(loadAtStartScenes);
    }
}