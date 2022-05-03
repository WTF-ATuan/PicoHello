using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLoading : MonoBehaviour
{
    int sceneNum;
    AsyncOperation _asyncOperation;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public int TargetSceneName
    {
        set
        {
            sceneNum = value;
            StartCoroutine(LoadScene(sceneNum));
        }
    }

    IEnumerator LoadScene(int sceneNum)
    {
        yield return new WaitForSeconds(1.2f);
        _asyncOperation = SceneManager.LoadSceneAsync(sceneNum);

        while (!_asyncOperation.isDone)
        {
            float p = _asyncOperation.progress * 100 + 10f;
            if (p > 100) p = 100;
            yield return null;
        }
        while (_asyncOperation.isDone)
        {
            Invoke("DesObj", 5);
            break;
        }
    }

    [System.Obsolete]
    public void DesObj()
    {
        DestroyObject(gameObject);
    }

}
