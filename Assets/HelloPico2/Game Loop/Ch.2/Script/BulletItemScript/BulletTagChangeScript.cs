using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTagChangeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.tag ="ball";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.tag = "ballAttack";
        }
    }
}
