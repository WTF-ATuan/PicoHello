using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HitCageCh3Controller : MonoBehaviour
{
    public GameObject[] getTriggetList;
    public Animator TragetCage;
    public bool isShow;
    public float randomTime;
    public float maxTime;
    float timeCount;
    float nextActiveTime;
    int randomRangeValue;
    int randomRangeTriggetValue;
    int triggetListLength;
    // Start is called before the first frame update
    void Start()
    {
        triggetListLength = getTriggetList.Length;
        foreach (GameObject setTriggetObj in getTriggetList)
        {
            setTriggetObj.GetComponent<SphereCollider>().enabled = false;
            setTriggetObj.SetActive(false);
        }
        nextActiveTime = randomTime;
        
    }

    // Update is called once per frame
        void Update()
    {
        if (isShow && timeCount < maxTime)
        {
            SetRandomActive();
        }
        if(timeCount < maxTime)
        {
            timeCount += Time.deltaTime;
            if (timeCount > nextActiveTime)
            {
                SetRandomActiveHide();
                nextActiveTime += randomTime;
            }
        }
        if(timeCount > maxTime)
        {
            CloseController();
        }
    }
    void CloseController()
    {
        TragetCage.SetTrigger("Release");
        gameObject.SetActive(false);
    }
    void  SetRandomActiveHide()
    { 
        foreach (GameObject setTriggetObj in getTriggetList)
        {
            OnScaleToZero(setTriggetObj);
        }
        StartCoroutine( WaitShowObj());
    }
    void SetRandomActive()
    {
        
        randomRangeValue = Random.Range(1, 3);
        if (randomRangeValue == 1)
        {
            randomRangeTriggetValue = Random.Range(0, triggetListLength + 1);
            getTriggetList[randomRangeTriggetValue].SetActive(true);
            OnScaleToStar(getTriggetList[randomRangeTriggetValue]);
            isShow = false;
            
        }
        else
        {
            randomRangeTriggetValue = Random.Range(0, triggetListLength + 1);
            getTriggetList[randomRangeTriggetValue].SetActive(true);
            OnScaleToStar(getTriggetList[randomRangeTriggetValue]);
            randomRangeTriggetValue = Random.Range(0, triggetListLength + 1);
            getTriggetList[randomRangeTriggetValue].SetActive(true);
            OnScaleToStar(getTriggetList[randomRangeTriggetValue]);
            isShow = false;
            
        }
    }
    private void OnScaleToZero(GameObject getScaleObj )
    {
        getScaleObj.transform.DOScale(0, 0.3f)
            .SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                getScaleObj.GetComponent<SphereCollider>().enabled = false;
                getScaleObj.SetActive(false);
            });
    }
    private void OnScaleToStar(GameObject getScaleObj)
    {
        getScaleObj.transform.DOScale(1, 0.3f)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                getScaleObj.GetComponent<SphereCollider>().enabled = true;
            });
    }
    IEnumerator WaitShowObj()
    {
        yield return new WaitForSeconds(1f);
        isShow = true;
    }
}
