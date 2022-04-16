using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackerPointTimeScript : MonoBehaviour
{
    public GameObject TrackerPointL;
    public GameObject TrackerPointR;
    public int minTime=3 ;
    public int maxTime=30;
    int showTime;
    bool isWaiting = true;
    float timeCount;

    // Start is called before the first frame update
    void Start()
    {
        timeCount = 30;
    }

    // Update is called once per frame
    void Update()
    {
        timeCount = timeCount - Time.deltaTime;
        if (timeCount <= 0)
        {
            TrackerPointL.SetActive(!isWaiting);
            TrackerPointR.SetActive(isWaiting);
            isWaiting = !isWaiting;
            ChangeTime();

        }
    }
    private void ChangeTime()
    {
        showTime = Random.Range(minTime, maxTime);
        timeCount = showTime;
    }
}