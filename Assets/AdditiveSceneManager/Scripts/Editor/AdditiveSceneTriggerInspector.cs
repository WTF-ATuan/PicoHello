using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;

[CustomEditor(typeof(AdditiveSceneTrigger))]
public class AdditiveSceneTriggerInspector : Editor
{      
    private SerializedObject targetSerializedObject;
    private SerializedProperty visibleScenes;

    private bool addMode = false;
    
    void OnEnable()
    {
        targetSerializedObject = new SerializedObject(target);
        visibleScenes = targetSerializedObject.FindProperty("visibleScenes");
    }    

    public override void OnInspectorGUI()
    {
        targetSerializedObject.Update();

        RenderSectionHeader("Visible Scenes:");
        RenderSceneListPropertyField(visibleScenes);
        EditorGUILayout.Space();
                
        targetSerializedObject.ApplyModifiedProperties();    
    }

    void RenderSectionHeader(string header, string tooltip = "")
    {
        EditorGUILayout.LabelField(new GUIContent(header, tooltip), EditorStyles.largeLabel, GUILayout.Height(20f));
    }

    void RenderSceneListPropertyField(SerializedProperty listProperty)
    {
        for (var i = 0; i < listProperty.arraySize; i++)
        {
            var elementProperty = listProperty.GetArrayElementAtIndex(i);
            elementProperty.stringValue = RenderSceneListPropertyElement(elementProperty.stringValue, i, listProperty);
            if (String.IsNullOrEmpty(elementProperty.stringValue))
            {
                listProperty.DeleteArrayElementAtIndex(i);                                
                // we stop here, and Unity will draw the inspector again straight away with the updated array
                return;
            }
        }

        if (addMode)
        {
            var newSceneName = RenderSceneListPropertyElement(null, listProperty.arraySize, listProperty);
            if (!String.IsNullOrEmpty(newSceneName))
            {
                listProperty.arraySize++;
                listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1).stringValue = newSceneName;
                addMode = false;                
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
            addMode = false;            
        }
        EditorGUILayout.EndHorizontal();
        return sceneAsset != null ? sceneAsset.name : "";
    }

    SceneAsset GetSceneAsset(string sceneName)
    {
        if (String.IsNullOrEmpty(sceneName)) return null;
        var sceneGUID = AssetDatabase.FindAssets(String.Format("{0} t:scene", sceneName)).FirstOrDefault();
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
            addMode = true;
        }
    }
}