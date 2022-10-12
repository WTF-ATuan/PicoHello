using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passageway : MonoBehaviour
{
    public GameObject Cloud;
    public GameObject NextLevel;
    public float Equal_Z;
    public GameObject Close_Level;
    
    void Update()
    {
        if(transform.position.z <= Equal_Z)
        {
            NextLevel.SetActive(true);
            GetComponent<Passageway>().enabled = false;

            Cloud.transform.parent = NextLevel.transform;
            transform.parent = NextLevel.transform;

            Close_Level.SetActive(false);
        }
    }
}
