using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    float updateInterval = 1;
    float accum = 0;
    int frames = 0;
    float timeleft;
    string fpsFormat;



    void Start()
    {
        timeleft = updateInterval;
    }


    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if(timeleft<=0)
        {
            float fps = accum / frames;
            fpsFormat = System.String.Format("FPS: {0:F0}", fps);
            timeleft = updateInterval;
            accum = .0f;
            frames = 0;
        }

        GetComponent<Text>().text = fpsFormat;
    }
}
