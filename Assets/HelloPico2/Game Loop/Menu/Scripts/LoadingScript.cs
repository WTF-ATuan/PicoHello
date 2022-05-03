using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    public int sceneNum;
    public float loadCheckTime;
    AsyncOperation async;


    
    /*
    private void Start()
    {
        StartCoroutine(AsynloadScene());
    }
    IEnumerator AsynloadScene()
    {
        async = SceneManager.LoadSceneAsync(sceneNum);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            yield return null;
        }
        while (isChangeScene)
        {
            async.allowSceneActivation = true;
        }
        
        
        yield return null;
    }
        //¤Á´«³õ´º
    */
    public void LoadScene()
    {
        Invoke("seconeChange", loadCheckTime);
        
    }
    public void seconeChange()
    {
        //isChangeScene = true;
        
        SceneManager.LoadScene(sceneNum);
    }

}
