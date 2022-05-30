using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public partial class SceneControllerInspector
{
    Color[] currentlyLoadedSceneColors = new Color[] 
    {  
        new Color(1f, 0f, 0f),            // progress < 0.9f
        new Color(1f, 1f, 0f),          // progress < 1.0f
        new Color(0f, 1f, 0f)             // progress = 1.0f
    };

    Color defaultBackgroundColor = default(Color);
    
    private bool allowSceneActivation;

    void RenderCurrentlyLoadedScenes()
    {
        RenderSectionHeader("Currently Loaded Scenes", "Unity's SceneManager index of scenes");
        defaultBackgroundColor = GUI.backgroundColor;
        int inspectorSceneCount = 0; // Count how many scenes the inspector is displaying
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {            
            RenderCurrentlyLoadedScene(SceneManager.GetSceneAt(i), i);
            inspectorSceneCount++;
        }
        GUI.backgroundColor = defaultBackgroundColor;
        RenderLoadScene();
        EditorGUILayout.Space();
        //if (sceneController.asyncOperations.Count > 0) Repaint();
        if (inspectorSceneCount != sceneController.fullyLoadedScenes.Count) Repaint();
    }

    void RenderLoadScene()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.PrefixLabel("Load New Scene:");
        var newScene = EditorGUILayout.ObjectField(null, typeof(SceneAsset), false);
        GUILayout.Space(24);
        EditorGUILayout.EndHorizontal();

        if (newScene != null)
        {
            LoadScene(newScene.name);
        }        
    }

    void RenderCurrentlyLoadedScene(Scene scene, int position)
    {
        bool showActivateToggle = false;
        bool showUnloadButton = true;
        float progress = 1.0f;
        if (sceneController.asyncOperations.ContainsKey(scene.name)) {
            showActivateToggle = true;
            showUnloadButton = false;
            progress = sceneController.asyncOperations[scene.name].progress;
        }
        if (progress < 0.9f) {
            GUI.backgroundColor = currentlyLoadedSceneColors[0];
        }
        else if (progress < 1.0f) {
            GUI.backgroundColor = currentlyLoadedSceneColors[1];
        }
        else {
            GUI.backgroundColor = currentlyLoadedSceneColors[2];
        } 
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.PrefixLabel("Scene " + (position));
        EditorGUI.BeginDisabledGroup(true);
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
        EditorGUILayout.ObjectField(sceneAsset, typeof(SceneAsset), false);
        if(position != 0) EditorGUI.EndDisabledGroup();
        GUI.backgroundColor = defaultBackgroundColor;

        if (showUnloadButton) {
            if (GUILayout.Button(new GUIContent("-", "Unload this scene"), GUILayout.Width(20))) {
                UnloadScene(scene.name);
            }
        }
        if (showActivateToggle) {
            allowSceneActivation = sceneController.asyncOperations[scene.name].allowSceneActivation;
            sceneController.asyncOperations[scene.name].allowSceneActivation = GUILayout.Toggle(allowSceneActivation, new GUIContent("", "Allow Scene Activation"), GUILayout.Width(20));
        }

        if (position == 0) EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
    }

}