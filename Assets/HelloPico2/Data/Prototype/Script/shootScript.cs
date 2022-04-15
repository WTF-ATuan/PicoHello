using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootScript : MonoBehaviour
{
    public float speed=20f;

    void Update()
    {
            transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
            Destroy(gameObject, 5f);
       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "target")
        {
            Destroy(gameObject);
        }
    }
}
