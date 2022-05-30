using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class ProjectSceneList
{
    private static SceneControllerSettingsObject settings;    

    static ProjectSceneList()
    {        
        settings = Resources.Load<SceneControllerSettingsObject>("SceneControllerSettings");        
    }

    public static bool SceneExists(string sceneName)
    {                
        return settings.scenes.Contains(sceneName, StringComparer.OrdinalIgnoreCase);    
    }

    
    /// <returns>True if the scene list was changed and needs to be saved, false otherwise</returns>
    public static bool SetSceneList(List<string> newSceneList)
    {                
        if (!settings.scenes.SequenceEqual<string>(newSceneList))
        {
            settings.scenes = newSceneList;
            return true;
        }

        return false;
    }
}