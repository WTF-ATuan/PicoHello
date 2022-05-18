using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHand : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame


    
    private void OnTriggerEnter(Collider other)
    {
        other.tag = this.tag;
    }
}
