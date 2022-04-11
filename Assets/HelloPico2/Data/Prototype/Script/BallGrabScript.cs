using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGrabScript : MonoBehaviour
{
    public GameObject DeletSeat;
    public bool isDestroy = false;
    
    bool isKinCheck = false;
    // Start is called before the first frame update
    void Start()
    {
    
        if (isDestroy == true)
        {
            Destroy(gameObject, Random.Range(8.0f,16.0f));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(DeletSeat);
        }
        else if (other.tag == "target" || other.tag == "ground")
        {
            Destroy(this.gameObject);
        }
    }
}
