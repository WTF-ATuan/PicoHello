using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseRotation : MonoBehaviour
{

    public GameObject go;
    [Range(0,1)]
    public float Value;


    void Start()
    {
        
    }

    Vector3 currentRot;
    Vector3 deltaRot;
    Vector3 lastRot = new Vector3(0, 0, 0);


    void Update()
    {
       // Vector3 go_angle = go.transform.rotation.eulerAngles;

        currentRot = Camera.main.transform.rotation.eulerAngles ;
        deltaRot = currentRot - lastRot;
        lastRot = currentRot;


        //    transform.Rotate(-deltaRot* Value, Space.Self);


        transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position, Value);


        transform.rotation =

        Quaternion.Lerp(

            transform.rotation,

            Quaternion.RotateTowards
            (
            transform.rotation, 
            Quaternion.Euler(transform.rotation.eulerAngles - deltaRot),
            Value),

            Value

         );
        

        /*
        if(transform.rotation.eulerAngles.x<0)
        {
            transform.rotation = Quaternion.Euler(270 + transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
        if (transform.rotation.eulerAngles.y < 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 270 + transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
        if (transform.rotation.eulerAngles.z < 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 270 + transform.rotation.eulerAngles.z);
        }*/
    }

}
