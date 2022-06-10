using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideSingleControllerScript : MonoBehaviour
{
    public GameObject storyGuide;
    public GameObject interactableGuide;
    // Start is called before the first frame update
    
    public void GuideChangeToInteractable()
    {
        interactableGuide.SetActive(true);
        storyGuide.SetActive(false);
    }
    public void GuideChangeToStory()
    {
        storyGuide.SetActive(true);
        interactableGuide.SetActive(false);
    }
}

