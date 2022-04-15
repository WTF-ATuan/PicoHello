using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class typeChangeScript : MonoBehaviour
{
    public GameObject typeManager;
    public bool defType;
    bool currType;

    private void Start()
    {
        currType = defType;
    }
    private void Update()
    {
        if (typeManager.activeSelf == true)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {   
            typeManager.SetActive(!currType);
            currType = !currType;
        }
    }
}
