using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class TutorialSpawner : MonoBehaviour
{
    public GameObject insObj;
    public GameObject createPos;
    public GameObject moveGrp;
    public float waitTime=3;
    float timer;
    public bool isWait;
    [BoxGroup("NEW")]
    public float fadeinDuring = 0.5f;
    // Start is called before the first frame update

    public void Start()
    {
        if (!isWait)
        {
            CheckMoveType();
        }
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
    public void createMpveObj()
    {
        if (createPos.transform.childCount < 1)
        {
            GameObject objGrp = Instantiate(moveGrp, createPos.transform.position, Quaternion.identity, createPos.transform);
            GameObject obj = Instantiate(insObj, objGrp.transform.position, Quaternion.identity, objGrp.transform);
            var originScale = objGrp.transform.localScale;
            objGrp.transform.DOScale(Vector3.zero, 0); //將其物件先設成最小Scale;
            objGrp.transform.DOScale(originScale, fadeinDuring).SetEase(Ease.Linear);
        }
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            CheckMoveType();
            timer = 0;
        }
    }
    private void CheckMoveType()
    {
        if (moveGrp == null)
        {
            createObj();
        }
        else
        {
            createMpveObj();
        }
    }

}
