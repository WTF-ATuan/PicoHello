using UnityEngine;
using UnityEditor;

public static class HiddenMenuItem 
{
    const string k_menu = "Hierarachy_Hide/Show Hidden Objects";

    [MenuItem(k_menu)]
    static void ShowHiddenMenuItem()
    {
        EditorPrefs.SetBool(Constants.HIDDEN_FLAG, !EditorPrefs.GetBool(Constants.HIDDEN_FLAG, false));

            Hidden hidden = new Hidden();
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