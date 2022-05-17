using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTag : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(this.transform.name != "RightHand Controller")
        {
            other.transform.tag = "Untagged";
        }
        else
        {
            other.transform.tag = this.transform.name;
        }
    }
}
