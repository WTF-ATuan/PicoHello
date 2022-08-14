using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRoundControlScript : MonoBehaviour
{
    Animator roundAnimator;
    // Start is called before the first frame update
    void Start()
    {
        roundAnimator = gameObject.GetComponent<Animator>();
        roundAnimator.SetBool("isGet", true);
    }
    
    
}
