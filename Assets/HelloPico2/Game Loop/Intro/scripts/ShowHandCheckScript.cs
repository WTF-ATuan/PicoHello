using HelloPico2.LevelTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHandCheckScript : MonoBehaviour
{
    public GameObject showHandCheck;
    public bool isTouchCheck;
    public bool isHiFive;
    public float touchTime;
    float Timer;
    public GameObject getGuide;
    Animator guideAnimator;
    public string triggetName;

    private void Start()
    {
        Timer = 0;
        if (isTouchCheck)
        {
            guideAnimator = getGuide.GetComponent<Animator>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isTouchCheck ==true && isHiFive == false)
        {
            guideAnimator.SetTrigger(triggetName);
            showHandCheck.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isHiFive && other.name == "GetHand")
        {
            Timer += Time.deltaTime;
        }
        if (!isTouchCheck)
        {
            showHandCheck.SetActive(true);
            showHandCheck.GetComponent<ShackingDetector>().enabled = true;
        }
        if (isTouchCheck == true && isHiFive == true && Timer>touchTime)
        {
            guideAnimator.SetTrigger(triggetName);
            showHandCheck.SetActive(true);
            Timer = 0;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (!isTouchCheck)
        {
            showHandCheck.SetActive(false);
            showHandCheck.GetComponent<ShackingDetector>().enabled = false;
        }
    }
}
