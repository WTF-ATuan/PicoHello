using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLevel : MonoBehaviour
{

    public GameObject Protal;
    
    public GameObject RT;

    public GameObject Level_Space_Old;
    public GameObject Level_Space_New;

    void Start()
    {
        
    }


    void Update()
    {
        Protal.transform.Translate(new Vector3(0,0,-20*Time.deltaTime));

        if (RT.transform.position.z<transform.position.z)
        {
            RT.SetActive(false);
            Level_Space_New.transform.position = new Vector3(0, 0, 0);
            Level_Space_Old.SetActive(false);
        }
    }
}
