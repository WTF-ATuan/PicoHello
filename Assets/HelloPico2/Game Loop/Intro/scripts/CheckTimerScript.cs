using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTimerScript : MonoBehaviour
{
    public  float CountTimme = 12f;
    private float Timer;
    public GameObject showTip;
    public GameObject hideList;
    // Start is called before the first frame update
    void Start()
    {
        Timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        //Debug.Log(Timer);
        //Debug.Log(gameObject.name);
        if (Timer > CountTimme)
        {
            showTip.SetActive(true);
            hideList.SetActive(false);
            Timer = 0;
        }
    }
}
