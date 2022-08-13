using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class TutorialSpawner : MonoBehaviour
{
    public GameObject insObj;
    public GameObject createPos;
    public float waitTime=3;
    float timer;
    [BoxGroup("NEW")]
    public float fadeinDuring = 0.5f;
    // Start is called before the first frame update

    public void Start()
    {
        createObj();
        timer = 0;
    }
    public void createObj()
    {
        if (createPos.transform.childCount < 1)
        {
            GameObject obj = Instantiate(insObj, createPos.transform.position, Quaternion.identity, createPos.transform);
            var originScale = obj.transform.localScale;
            obj.transform.DOScale(Vector3.zero, 0); //將其物件先設成最小Scale;
            obj.transform.DOScale(originScale, fadeinDuring).SetEase(Ease.Linear);
        }
    }
    IEnumerator WaitCreate()
    {
        yield return new WaitForSeconds(waitTime);
        createObj();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            createObj();
            timer = 0;
        }
    }

}
