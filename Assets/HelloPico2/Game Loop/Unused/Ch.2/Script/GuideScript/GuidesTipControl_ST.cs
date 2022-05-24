using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidesTipControl_ST : MonoBehaviour
{
    public TargetItem_SO _getIntro;
    public GuideSys_SO _getGuide;
    public int setGuideType;
    public bool isOnlyPlayAnim;
    public bool isCheck;
    public int checkItemHeld;
    public float coldTime;
    public GameObject NextStep;
    [TextArea]
    public string Note;

    float timer;
    public bool isDefaultSetting;


    // Start is called before the first frame update

    private void Start()
    {

        _getGuide.guidesType = setGuideType;
        _getIntro.targetItemHeld = 0;

        
    }
    private void Update()
    {
        if (isOnlyPlayAnim)
        {
            AfterAnim();
        }


        if (isCheck && _getIntro.targetItemHeld == checkItemHeld)
        {
            showNext();
            gameObject.SetActive(false);
        }


        
    }
    public void OpenTipGrp() 
    {
        _getIntro.isTipL = true;
        _getIntro.isTipR = true;
        _getIntro.isTipOpen = true;
    }
    public void showNext()
    {
        NextStep.gameObject.SetActive(true);
    }
    void AfterAnim()
    {
        timer += Time.deltaTime;
        if (timer > coldTime)
        {
            _getGuide.guidesType = 0;
            if (isCheck)
            {
                OpenTipGrp();
            }
            else
            {
                showNext();
            }
            timer = 0;
            isOnlyPlayAnim = false;
         
        }
    }
    
    void setDefault()
    {
        _getIntro.targetItemHeld = 0;
        _getIntro.isTipOpen = false;
        _getIntro.isTipR = true;
        _getIntro.isTipL = true;
    }
}
