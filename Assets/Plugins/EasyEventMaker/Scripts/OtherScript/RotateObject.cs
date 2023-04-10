using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {

public float rotateX = 0.0f;
public float rotateY = 5.0f;
public float rotateZ = 0.0f;

    private void Awake()
    {
        rotateX = Random.Range(-200,200);
        rotateY = Random.Range(-600, 600);
        rotateZ = Random.Range(-300, 300);
    }



    void Update(){
		transform.Rotate(rotateX*Time.deltaTime,rotateY*Time.deltaTime,rotateZ*Time.deltaTime);
	}
}
