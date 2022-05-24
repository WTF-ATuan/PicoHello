using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigget : MonoBehaviour
{
    public bool isTrue;
    public GameObject[] EventTL;
    // Start is called before the first frame update
    void Start()
    {
        if (isTrue)
        {
            EventTL[0].SetActive(true);
        }
        else
        {
            EventTL[1].SetActive(false);
        }
        
    }


}
