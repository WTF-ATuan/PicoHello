using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMeetGuideScript : MonoBehaviour
{
    int meetTypeRandom;
    public GameObject[] meetList;
    public Animator guideAnimator;
    public bool isTest;
    public int setShow;
    // Start is called before the first frame update
    void Start()
    {
        guideAnimator = guideAnimator.GetComponent<Animator>();
        if (!isTest)
        {
            meetTypeRandom = Random.Range(0, 2);
            meetList[meetTypeRandom].SetActive(true);
            guideAnimator.SetTrigger(meetList[meetTypeRandom].name);
        }
        else
        {
            meetList[setShow].SetActive(true);
            guideAnimator.SetTrigger(meetList[setShow].name);
        }
        
    }

}
