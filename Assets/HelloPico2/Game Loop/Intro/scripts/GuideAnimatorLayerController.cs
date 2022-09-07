using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideAnimatorLayerController : MonoBehaviour
{
    //public GameObject getGuideAnimator;
    public Animator guideAnimator;
    public int setLayerWeight;
    // Start is called before the first frame update
    void Start()
    {
        guideAnimator.SetLayerWeight(1, setLayerWeight);

    }

}
