using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMoveShootScript : MonoBehaviour
{
    public float speed=20f;
    private int type = 1;// 0 Default ,1 create ball move
                        // Update is called once per frame

    private void Start()
    {
        if (type == 1)
        {
            speed = Random.Range(10, speed);
        }
    }
    void Update()
    {
        if (type == 0)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
        }
        else
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed, Space.Self);
            Destroy(gameObject, Random.Range(5,10));
        }
 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            type = 0;
        }
        if (other.tag == "grounad")
        {
            Destroy(gameObject);
        }
    }
}
