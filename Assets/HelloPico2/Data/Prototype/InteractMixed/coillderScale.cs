using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coillderScale : MonoBehaviour
{
    private SphereCollider _sphereColl;
    // Start is called before the first frame update
    void Start()
    {
        _sphereColl = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x < 0.001f)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _sphereColl.radius = 0.05f;
            CancelInvoke("destObj");
        }
    }
    public void OnTriggerExit(Collider other)
    {
        GetComponent<SphereCollider>().radius = 0.4f;
        Invoke("destObj", 5f);
    }
    private void destObj()
    {
        Destroy(gameObject, 5f);
    }
}
