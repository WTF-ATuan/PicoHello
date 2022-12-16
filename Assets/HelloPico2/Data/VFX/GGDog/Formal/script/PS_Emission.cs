using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_Emission : MonoBehaviour
{
    public enum Emission
    {
        On_Emission, Off_Emission
    }
    public Emission emission;

    ParticleSystem PS;

    void Start()
    {
        PS = transform.GetComponent<ParticleSystem>();
    }

    int pre_switch = 0;
    int now_switch = 0;
    void Update()
    {
        var em = PS.emission;
        switch (emission)
        {
            case Emission.On_Emission:
                now_switch = 0;
                if (pre_switch != now_switch)
                {
                    em.enabled = true;
                    pre_switch = now_switch;
                }
                break;

            case Emission.Off_Emission:
                now_switch = 1;
                if (pre_switch != now_switch)
                {
                    em.enabled = false;
                    pre_switch = now_switch;
                }
                break;
        }
    }
}
