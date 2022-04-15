using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackerPointTimeScript : MonoBehaviour
{
    public GameObject TrackerPointL;
    public GameObject TrackerPointR;
    int showTime;
    bool isWaiting = true;
    float timeCount;

    // Start is called before the first frame update
    void Start()
    {
        timeCount = 20;
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
        showTime = Random.Range(3, 30);
        timeCount = showTime;
    }
}