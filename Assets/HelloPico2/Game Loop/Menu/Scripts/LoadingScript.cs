using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    public TargetItem_SO ItemHeld;
    private int HeldDefault = 0;
    public void Start()
    {
        if(ItemHeld.targetItemHeld!=0)
        {
            LoadFun();
        }
        
    }

    private void LoadFun()
    {
        SceneManager.LoadScene(0);
        ItemHeld.targetItemHeld = 0;
        gameObject.SetActive(false);
    }
    /*
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


}
