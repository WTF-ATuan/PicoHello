using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveScript : MonoBehaviour
{
    //public GameObject targetOb;
    public float speed = 0.01f;
    float timer=0;
    public float desTime=10.0f;
    public int moveAxis;//xyz
    
    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;
        Vector3 yMove = transform.up;
        Vector3 xMove = transform.right;
        Vector3 zMove = -transform.forward;
        Vector3 negyMove = -transform.up;
        if (moveAxis == 0)
        {
            transform.position += xMove * timer * speed;
        }
        else if(moveAxis == 1)
        {
            transform.position += yMove * timer * speed;
        }
        else if(moveAxis ==2)
        {
            transform.position += zMove * timer * speed;
        }
        else if (moveAxis == 3)
        {
            
            transform.position += negyMove * timer * speed;
        }
        if (timer > desTime && desTime !=0)
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
