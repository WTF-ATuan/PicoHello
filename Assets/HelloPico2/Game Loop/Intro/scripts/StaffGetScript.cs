using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffGetScript : MonoBehaviour
{
    public GameObject showObject;
    private void OnTriggerEnter(Collider other)
    {
         if(CompareTag("Player"))
         {
            showObject.SetActive(true);
         }
    }
}
