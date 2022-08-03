using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour {
	public float speed = 20;
	public Vector3 relativeDirection = Vector3.forward;

    // Update is called once per frame
    private void Start()
    {
        speed = Random.Range(1, speed);
    }
    void Update () {
		Vector3 absoluteDirection = transform.rotation * relativeDirection;
		transform.position += absoluteDirection * speed * Time.deltaTime;
	}
}
