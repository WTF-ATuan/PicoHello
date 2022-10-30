using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideTriggetCheckScript : MonoBehaviour
{
    public GameObject getGuideAnimator;
    public string triggerName;
    Animator guideAnimator;
    BoxCollider closeCollider;    
    
    // Start is called before the first frame update
    void Start()
    {
        guideAnimator = getGuideAnimator.GetComponent<Animator>();
        closeCollider = this.GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        guideAnimator.SetTrigger(triggerName);
        closeCollider.enabled = false;
    }


}
