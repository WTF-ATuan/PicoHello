using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallSceneScript : MonoBehaviour
{
    public GameObject[] checkList;
    public GameObject fabeObj;
    public int sceneInt;
    public float coldTimer;
    float countTime;
    public bool isCollider;
    public void callScene()
    {
        StartCoroutine(LoadNext());
    }
    
    IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(coldTimer);
        GameObject go = Instantiate(fabeObj);
        go.GetComponent<AsyncLoading>().TargetSceneName = sceneInt;
        yield return null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isCollider)
        {
            StartCoroutine(LoadNext());
            isCollider = false;
        }
    }
}
