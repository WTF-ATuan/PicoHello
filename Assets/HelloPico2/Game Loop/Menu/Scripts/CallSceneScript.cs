using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallSceneScript : MonoBehaviour
{
    public GameObject[] checkList;
    public GameObject fabeObj;
    public int sceneInt;
    public void callScene()
    {
        if (checkList[0] == null)
        {
            GameObject go = Instantiate(fabeObj);
            go.GetComponent<AsyncLoading>().TargetSceneName = sceneInt;
        }
        
    }
}
