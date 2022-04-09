using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGrabScript : MonoBehaviour
{
    public bool isDestroy = false;
    bool isKinCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        if (isDestroy == true)
        {
            Destroy(gameObject, 10);
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
            ChangeKin();
        }
        else if(other.tag == "target" || other.tag =="ground")
        {
            Destroy(this.gameObject);
        }
    }
    void ChangeKin()
    {
        this.GetComponent<Rigidbody>().isKinematic = false;
    }
}
