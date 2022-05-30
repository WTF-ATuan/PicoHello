using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public partial class SceneControllerInspector {

    void RenderLightmapOptions() {
        // Bake All Open Scenes button
        if (GUILayout.Button(new GUIContent("Bake Lightmaps On All Open Scenes"))) {
            string[] paths = new string[SceneManager.sceneCount];
            int sceneCount = 0;
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                paths[i] = SceneManager.GetSceneAt(i).path;
                sceneCount++;
            }
            if (EditorUtility.DisplayDialog("Bake lightmaps on all open scenes?", "Are you sure you want to bake lighting on " + sceneCount + " scenes? This may take a while and Unity Editor is unresponsive while baking. Lightmap Settings from the active scene will be used.", "OK", "Cancel")) {
                Lightmapping.BakeMultipleScenes(paths);
            }
        }
    }
}
