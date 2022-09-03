using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLinePipeLine : MonoBehaviour
{
    Level_FadeController setLevelEnv;
    public GameObject selectEnv;
    public GameObject findEnvLevel4;
    public GameObject findEnvLevel5;
    public GameObject setTunnel;
    public enum timeLineType
    {
        build ,
        intro,
        ch1,
        ch2,
        ch3,
        ch4
    }
    public timeLineType setTimeLineType;

    [Header("---------------------------------------")]
    public logoTimeline LogoTimeline;
    #region
    [System.Serializable]
    public struct logoTimeline
    {
        public GameObject[] showLogoList;
        public GameObject[] hideLogoList;
    }
    #endregion
    [Header("---------------------------------------")]
    public introTimeline IntroTimeline;
    #region
    [System.Serializable]
    public struct introTimeline
    {
        
        public GameObject[] showIntroList;
        public GameObject[] hideIntroList;
        
    }
    #endregion
    [Header("---------------------------------------")]
    public ch1Timeline Ch1Timeline;
    #region
    [System.Serializable]
    public struct ch1Timeline
    {
        public GameObject[] showCh1List;
        public GameObject[] hideCh1List;
    }
    #endregion
    [Header("---------------------------------------")]
    public ch2Timeline Ch2Timeline;
    #region
    [System.Serializable]
    public struct ch2Timeline
    {
        public GameObject[] showCh2List;
    }
    #endregion
    [Header("---------------------------------------")]
    public ch3Timeline Ch3Timeline;
    #region
    [System.Serializable]
    public struct ch3Timeline
    {
        public GameObject[] showCh3List;
        public GameObject[] hideCh3List;
    }
    #endregion
    [Header("---------------------------------------")]
    public ch4Timeline Ch4Timeline;
    #region
    [System.Serializable]
    public struct ch4Timeline
    {
        public GameObject[] showCh4List;
    }
    #endregion
    void TimePipeSetting_switch()
    {
        switch (setTimeLineType)
        {
            case timeLineType.build:
                findEnvLevel5.SetActive(false);
                findEnvLevel4.SetActive(false);
                setLevelEnv.level = Level_FadeController.Level._0_Level_Intro;
                setLevelEnv.Enable_Env = true;

                ShowObjFun(LogoTimeline.showLogoList);
                hideObjFun(LogoTimeline.hideLogoList);

                hideObjFun(IntroTimeline.showIntroList);
                hideObjFun(IntroTimeline.hideIntroList);
                ShowTLFun(IntroTimeline.showIntroList[0]);

                hideObjFun(Ch1Timeline.showCh1List);
                hideObjFun(Ch1Timeline.hideCh1List);
                ShowTLFun(Ch1Timeline.showCh1List[0]);

                hideObjFun(Ch2Timeline.showCh2List);
                ShowTLFun(Ch2Timeline.showCh2List[0]);

                hideObjFun(Ch3Timeline.showCh3List);
                hideObjFun(Ch3Timeline.hideCh3List);
                ShowTLFun(Ch3Timeline.showCh3List[0]);

                hideObjFun(Ch4Timeline.showCh4List);
                ShowTLFun(Ch4Timeline.showCh4List[0]);
                break;

            case timeLineType.intro:

                findEnvLevel5.SetActive(false);
                findEnvLevel4.SetActive(false);
                setLevelEnv.level = Level_FadeController.Level._0_Level_Intro;
                setLevelEnv.Enable_Env = true;

                hideObjFun(LogoTimeline.showLogoList);
                hideObjFun(LogoTimeline.hideLogoList);

                ShowObjFun(IntroTimeline.showIntroList);
                hideObjFun(IntroTimeline.hideIntroList);

                hideObjFun(Ch1Timeline.showCh1List);
                hideObjFun(Ch1Timeline.hideCh1List);
                ShowTLFun(Ch1Timeline.showCh1List[0]);

                hideObjFun(Ch2Timeline.showCh2List);
                ShowTLFun(Ch2Timeline.showCh2List[0]);

                hideObjFun(Ch3Timeline.showCh3List);
                hideObjFun(Ch3Timeline.hideCh3List);
                ShowTLFun(Ch3Timeline.showCh3List[0]);

                hideObjFun(Ch4Timeline.showCh4List);
                ShowTLFun(Ch4Timeline.showCh4List[0]);
                break;

            case timeLineType.ch1:

                findEnvLevel5.SetActive(false);
                findEnvLevel4.SetActive(false);
                setLevelEnv.level = Level_FadeController.Level._1_Level_1;
                setLevelEnv.Enable_Env = true;

                hideObjFun(LogoTimeline.showLogoList);
                hideObjFun(LogoTimeline.hideLogoList);

                hideObjFun(IntroTimeline.showIntroList);
                hideObjFun(IntroTimeline.hideIntroList);

                ShowObjFun(Ch1Timeline.showCh1List);
                hideObjFun(Ch1Timeline.hideCh1List);

                hideObjFun(Ch2Timeline.showCh2List);
                ShowTLFun(Ch2Timeline.showCh2List[0]);

                hideObjFun(Ch3Timeline.showCh3List);
                hideObjFun(Ch3Timeline.hideCh3List);
                ShowTLFun(Ch3Timeline.showCh3List[0]);

                hideObjFun(Ch4Timeline.showCh4List);
                ShowTLFun(Ch4Timeline.showCh4List[0]);
                break;

            case timeLineType.ch2:

                findEnvLevel5.SetActive(false);
                findEnvLevel4.SetActive(false);
                setLevelEnv.level = Level_FadeController.Level._2_Level_2;
                setLevelEnv.Enable_Env = true;

                hideObjFun(LogoTimeline.showLogoList);
                hideObjFun(LogoTimeline.hideLogoList);

                hideObjFun(IntroTimeline.showIntroList);
                hideObjFun(IntroTimeline.hideIntroList);

                hideObjFun(Ch1Timeline.showCh1List);
                hideObjFun(Ch1Timeline.hideCh1List);

                ShowObjFun(Ch2Timeline.showCh2List);

                hideObjFun(Ch3Timeline.showCh3List);
                hideObjFun(Ch3Timeline.hideCh3List);
                ShowTLFun(Ch3Timeline.showCh3List[0]);

                hideObjFun(Ch4Timeline.showCh4List);
                ShowTLFun(Ch4Timeline.showCh4List[0]);
                break;

            case timeLineType.ch3:

                findEnvLevel5.SetActive(false);
                findEnvLevel4.SetActive(false);
                setLevelEnv.level = Level_FadeController.Level._4_Level_3_ToReality;
                setLevelEnv.Enable_Env = false;

                hideObjFun(LogoTimeline.showLogoList);
                hideObjFun(LogoTimeline.hideLogoList);

                hideObjFun(IntroTimeline.showIntroList);
                hideObjFun(IntroTimeline.hideIntroList);

                hideObjFun(Ch1Timeline.showCh1List);
                hideObjFun(Ch1Timeline.hideCh1List);

                hideObjFun(Ch2Timeline.showCh2List);

                ShowObjFun(Ch3Timeline.showCh3List);
                hideObjFun(Ch3Timeline.hideCh3List);

                ShowTLFun(Ch4Timeline.showCh4List[0]);
                hideObjFun(Ch4Timeline.showCh4List);
                break;

            case timeLineType.ch4:

                setLevelEnv.level = Level_FadeController.Level._5_ToLevel_4_BackToGame;
                setLevelEnv.Enable_Env = true;

                hideObjFun(LogoTimeline.showLogoList);
                hideObjFun(LogoTimeline.hideLogoList);

                hideObjFun(IntroTimeline.showIntroList);
                hideObjFun(IntroTimeline.hideIntroList);

                hideObjFun(Ch1Timeline.showCh1List);
                hideObjFun(Ch1Timeline.hideCh1List);

                hideObjFun(Ch2Timeline.showCh2List);

                hideObjFun(Ch3Timeline.showCh3List);
                hideObjFun(Ch3Timeline.hideCh3List);

                ShowObjFun(Ch4Timeline.showCh4List);
                break;
        }
    }
    void ShowObjFun(GameObject[] showList)
    {
        foreach (GameObject showTimeObj in showList)
        {
            showTimeObj.SetActive(true);
        }
    }
    void hideObjFun(GameObject[] hideList)
    {
        foreach (GameObject hideTimeObj in hideList)
        {
            hideTimeObj.SetActive(false);
        }
    }
    void ShowTLFun(GameObject showTLObj)
    {
        showTLObj.SetActive(true);
    }
    private void Start()
    {
        setTunnel.transform.position = Vector3.zero;
        setLevelEnv = selectEnv.GetComponent<Level_FadeController>();
        TimePipeSetting_switch();
    }
}
