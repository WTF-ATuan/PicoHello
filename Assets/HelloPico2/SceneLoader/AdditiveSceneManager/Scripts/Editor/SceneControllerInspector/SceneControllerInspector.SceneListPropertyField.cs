using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public partial class SceneControllerInspector
{
    List<string> sceneListsInAddMode = new List<string>();

    void RenderSceneListPropertyField(SerializedProperty listProperty)
    {          
        for (var i = 0; i < listProperty.arraySize; i++)
        {
            var elementProperty = listProperty.GetArrayElementAtIndex(i);
            elementProperty.stringValue = RenderSceneListPropertyElement(elementProperty.stringValue, i, listProperty);
            if (String.IsNullOrEmpty(elementProperty.stringValue))
            {
                listProperty.DeleteArrayElementAtIndex(i);
                targetSerializedObject.ApplyModifiedProperties();
                settingsObject.ApplyModifiedProperties();
                GenerateSceneList();
                // we stop here, and Unity will draw the inspector again straight away with the updated array
                return;
            }
        }

        if (sceneListsInAddMode.Contains(listProperty.name))
        {
            var newSceneName = RenderSceneListPropertyElement(null, listProperty.arraySize, listProperty);
            if (!String.IsNullOrEmpty(newSceneName))
            {
                listProperty.arraySize++;
                listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1).stringValue = newSceneName;
                sceneListsInAddMode.Remove(listProperty.name); // leave add mode
                targetSerializedObject.ApplyModifiedProperties();
                settingsObject.ApplyModifiedProperties();
                GenerateSceneList();
            }
        }
        else
        {
            RenderSceneListAddSceneButton(listProperty);
        }
    }   

    string RenderSceneListPropertyElement(string sceneName, int positionInArray, SerializedProperty listProperty)
    {
        SceneAsset sceneAsset = GetSceneAsset(sceneName);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent(string.Format("Scene {0}", positionInArray)));
        sceneAsset = (SceneAsset)EditorGUILayout.ObjectField(sceneAsset, typeof(SceneAsset), false);
        var removeSceneFromList = GUILayout.Button(new GUIContent("-", "Remove this scene from the list"), GUILayout.Width(20));
        if (removeSceneFromList)
        {
            sceneAsset = null;
            if (sceneListsInAddMode.Contains(listProperty.name)) {
                sceneListsInAddMode.Remove(listProperty.name); // leave add mode
            }
            targetSerializedObject.ApplyModifiedProperties();
            settingsObject.ApplyModifiedProperties();
            GenerateSceneList();
        }
        EditorGUILayout.EndHorizontal();        
        return sceneAsset != null ? sceneAsset.name : "";
    }

    SceneAsset GetSceneAsset(string sceneName) {
        if (String.IsNullOrEmpty(sceneName)) return null;
        UnityEngine.Object scene = new UnityEngine.Object();
        string sceneGUID = "";
        string[] sceneGUIDs = AssetDatabase.FindAssets(String.Format("{0} t:scene", sceneName));
        foreach (string guid in sceneGUIDs) {
            scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(guid));
            if (scene.name == sceneName) {
                sceneGUID = guid;
            }
        }
        if (String.IsNullOrEmpty(sceneGUID)) return null;
        return AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(sceneGUID));
    }

    void RenderSceneListAddSceneButton(SerializedProperty property)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        bool enterAddMode = GUILayout.Button("Add Scene", GUILayout.Width(75));
        EditorGUILayout.EndHorizontal();
        if (enterAddMode)
        {
            if(!sceneListsInAddMode.Contains(property.name)) sceneListsInAddMode.Add(property.name);
        }
    }
}