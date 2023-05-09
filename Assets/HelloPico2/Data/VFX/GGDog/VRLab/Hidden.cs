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
#if UNITY_EDITOR
        bool hidden = UnityEditor.EditorPrefs.GetBool(Constants.HIDDEN_FLAG, false);
        this.gameObject.hideFlags = hidden ? HideFlags.HideInHierarchy : HideFlags.None;
#endif
    }

}

//Source: https://www.youtube.com/watch?v=LXq8qSeuSAc