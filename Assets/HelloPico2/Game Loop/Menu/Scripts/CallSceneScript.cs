using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallSceneScript : MonoBehaviour
{
    public GameObject[] showList;
    public GameObject[] hideList;
    

    public bool isCheck;
    
    public void Update()
    {
        if (!isCheck) return;
        foreach (GameObject showObjElement in showList)
        {
            showObjElement.SetActive(true);
        }
        foreach (GameObject hideObjElement in hideList)
        {
            hideObjElement.SetActive(false);
        }
        isCheck = false;
    }
    
    /*
    IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(coldTimer);
        GameObject go = Instantiate(fabeObj);
        go.GetComponent<AsyncLoading>().TargetSceneName = sceneInt;
        yield return null;
    }*/

}
