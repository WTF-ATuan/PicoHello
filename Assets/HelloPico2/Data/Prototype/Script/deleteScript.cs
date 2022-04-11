using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deleteScript : MonoBehaviour
{
    public float desMinTime=10.0f;
    public float desMaxTime=20.0f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, Random.Range(desMinTime,desMaxTime));
    }

}
