using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_BeamCharge_Controller : MonoBehaviour
{

    public GameObject Loop;
    public GameObject Shot;
    public GameObject Loop_InShot;

    public enum Controller
    {
        Start,Shot, End
    }
    public Controller controller;

    ParticleSystem[] PS_Loop = new ParticleSystem[50];
    ParticleSystem[] PS_Shot = new ParticleSystem[50];
    ParticleSystem[] PS_Loop_InShot = new ParticleSystem[50];

    void Awake()
    {
        for(int i=0; i<Loop.transform.childCount;i++)
        {
            PS_Loop[i] = Loop.transform.GetChild(i).GetComponent<ParticleSystem>();
        }
        for (int i = 0; i < Shot.transform.childCount; i++)
        {
            PS_Shot[i] = Shot.transform.GetChild(i).GetComponent<ParticleSystem>();
        }
        for (int i = 0; i < Loop_InShot.transform.childCount; i++)
        {
            PS_Loop_InShot[i] = Loop_InShot.transform.GetChild(i).GetComponent<ParticleSystem>();
        }
        Shot.gameObject.SetActive(false);
    }

    int pre_switch = 0;
    int now_switch = 0;
    void Update()
    {

        switch (controller)
        {

            case Controller.Start:
                now_switch = 0;
                if (pre_switch != now_switch)
                {
                    gameObject.SetActive(false);
                    gameObject.SetActive(true);
                    Shot.gameObject.SetActive(false);
                    PS_Shot[1].gameObject.SetActive(true);

                    EmissionControl(Loop.transform.childCount, PS_Loop, true);
                    EmissionControl(Loop_InShot.transform.childCount, PS_Loop_InShot, true);
                    pre_switch = now_switch;
                }
                break;

            case Controller.Shot:
                now_switch = 1;
                if (pre_switch != now_switch)
                {
                    //關掉集氣Loop
                    EmissionControl(Loop.transform.childCount, PS_Loop,false);

                    //打開發射Loop
                    EmissionControl(Shot.transform.childCount, PS_Shot, true);

                    Shot.gameObject.SetActive(true);

                    pre_switch = now_switch;
                }
                break;

            case Controller.End:
                now_switch = 2;
                if (pre_switch != now_switch)
                {
                    //關掉發射Loop
                    EmissionControl(Shot.transform.childCount, PS_Shot, false);
                    //關掉發射內的集氣Loop
                    EmissionControl(Loop_InShot.transform.childCount, PS_Loop_InShot, false);

                    //單獨關掉泡泡延遲太久的
                    PS_Shot[1].gameObject.SetActive(false);

                    pre_switch = now_switch;
                }
                break;
        }
    }


    void EmissionControl(int childCount, ParticleSystem[] PS,bool Bool)
    {
        for (int i = 0; i < childCount; i++)
        {
            var em = PS[i].emission;
            em.enabled = Bool;
        }
    }
}
