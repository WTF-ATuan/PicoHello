using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
    }
}
