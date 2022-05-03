using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveScript : MonoBehaviour
{
    public GameObject targetOb;
    float speed = 0.01f;
    float timer=0;
    
    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;
        Vector3 yMove = transform.up;
        transform.position += yMove * timer*speed;
        if (timer > 5f)
        {
            Destroy(gameObject);
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if(other.name== targetOb.name)
        {
            Destroy(gameObject);
        }
    }*/
}
