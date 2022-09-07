using HelloPico2.LevelTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHandCheckScript : MonoBehaviour
{
    public GameObject showHandCheck;
    public bool isTouchCheck;

    public GameObject getGuide;
    Animator guideAnimator;
    public string triggetName;

    private void Start()
    {
        if (isTouchCheck)
        {
            guideAnimator = getGuide.GetComponent<Animator>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isTouchCheck )
        {
            guideAnimator.SetTrigger(triggetName);
            showHandCheck.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isTouchCheck)
        {
            showHandCheck.SetActive(true);
            showHandCheck.GetComponent<ShackingDetector>().enabled = true;
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
