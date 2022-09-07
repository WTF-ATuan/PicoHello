using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideTriggetCheckScript : MonoBehaviour
{
    public GameObject getGuideAnimator;
    public GameObject colliderTriggerName;
    public string triggerName;
    Animator guideAnimator;
    // Start is called before the first frame update
    void Start()
    {
        guideAnimator = getGuideAnimator.GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == colliderTriggerName.name)
        {
            guideAnimator.SetTrigger(triggerName);
        }
    }


}
