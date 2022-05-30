using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class SceneListProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        GenerateSceneList();
    }

    public static void GenerateSceneList()
    {
        var scenes = AssetDatabase.FindAssets("t:scene")
                                  .Select(s => AssetDatabase.GUIDToAssetPath(s))
                                  .Select(s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s))
                                  .Select(s => s.name)
                                  .ToList();

        if (ProjectSceneList.SetSceneList(scenes))
        {            
            AssetDatabase.SaveAssets();
        }
    }
}
