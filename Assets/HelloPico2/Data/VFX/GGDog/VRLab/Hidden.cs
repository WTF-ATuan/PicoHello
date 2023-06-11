using UnityEngine;

public static class Constants
{
    public const string HIDDEN_FLAG = "HiddenFlag";
}

public class Hidden : MonoBehaviour
{
    private void OnValidate()
    {
        RefreshHiddenState();
    }

    public void RefreshHiddenState()
    {
        bool hidden = UnityEditor.EditorPrefs.GetBool(Constants.HIDDEN_FLAG, false);

        //Tag³]¬°"Hide"ªº
        foreach (GameObject h in GameObject.FindGameObjectsWithTag("Hide"))
            h.hideFlags = hidden ? HideFlags.HideInHierarchy : HideFlags.None;
    }

}

//Source: https://www.youtube.com/watch?v=LXq8qSeuSAc