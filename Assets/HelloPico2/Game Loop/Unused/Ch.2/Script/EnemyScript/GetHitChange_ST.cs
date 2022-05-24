using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHitChange_ST : MonoBehaviour
{
    public EnemyScore_SO _hitScoreSO;

    public GameObject[] ShowHitList;
    public GameObject[] ShowStepList;

    //public int StepCounter;
    public int[] HitStepList;
    public bool isSetController;

    bool isFinal;
    public GameObject[] CloseList;

    public GameObject[] AduioList;

    bool isGuideHit=true;
    bool isStepHit;

    void ShowHitOpen()
    {
        if (isGuideHit)
        {
            if (_hitScoreSO.getStepHit == 1)
            {
                ShowStepList[0].SetActive(true);
            }
            if (_hitScoreSO.getStepHit == 2)
            {
                ShowStepList[1].SetActive(true);
                AduioList[0].SetActive(true);
                isStepHit = true;
                isGuideHit = false;
            }
        }
        
        if (isStepHit)
        {
            if (_hitScoreSO.getHit > HitStepList[0])
            {
                ShowHitList[0].SetActive(true);
            }
            if (_hitScoreSO.getHit > HitStepList[1])
            {
                ShowHitList[1].SetActive(true);
                AduioList[1].SetActive(true);
            }
            if (_hitScoreSO.getHit > HitStepList[2])
            {
                ShowHitList[2].SetActive(true);
                ShowHitList[3].SetActive(true);
                isFinal = true;
            }
        }
    }
    void Final()
    {

        CloseList[0].SetActive(false);
        CloseList[1].SetActive(false);
        AduioList[0].SetActive(false);
        //CloseList[2].SetActive(true);
        Invoke("Close", 12);
    }
    void Close()
    {
        CloseList[2].SetActive(true);
        CloseList[3].SetActive(true);
        AduioList[1].SetActive(false);
        ShowHitList[0].SetActive(false);
        ShowHitList[1].SetActive(false);
        ShowHitList[2].SetActive(false);
        ShowHitList[3].SetActive(false);
        ShowStepList[0].SetActive(false);
        ShowStepList[1].SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (isSetController)
        {
            ShowHitOpen();
        }
        if (isFinal)
        {
            Final();
        }
    }
}
