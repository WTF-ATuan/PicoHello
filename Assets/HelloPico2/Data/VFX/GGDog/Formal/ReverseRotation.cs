using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseRotation : MonoBehaviour
{

    [Range(0,1)]
    public float Value;


    Vector3 currentRot;
    Vector3 deltaRot;
    Vector3 lastRot = new Vector3(0, 0, 0);


    void Update()
    {

        transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position, Value);


        currentRot = Camera.main.transform.rotation.eulerAngles ;
        deltaRot = currentRot - lastRot;
        lastRot = currentRot;


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
        
    }

}
