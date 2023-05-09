using UnityEngine;
using UnityEditor;

public static class HiddenMenuItem 
{
    const string k_menu = "GGDog Hierarachy/Show Hidden Objects";

    [MenuItem(k_menu)]
    static void ShowHiddenMenuItem()
    {
        EditorPrefs.SetBool(Constants.HIDDEN_FLAG, !EditorPrefs.GetBool(Constants.HIDDEN_FLAG, false));

        foreach (Hidden hidden in GameObject.FindObjectsOfType<Hidden>())
            hidden.RefreshHiddenState();
    }

    [MenuItem(k_menu,true)]
    static bool ShowHiddenMenuItemValidation()
    {
        Menu.SetChecked(k_menu, !EditorPrefs.GetBool(Constants.HIDDEN_FLAG, false));
        return true;
    }

}

//Source: https://www.youtube.com/watch?v=LXq8qSeuSAc