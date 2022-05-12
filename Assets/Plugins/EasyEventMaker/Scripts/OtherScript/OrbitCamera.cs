using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour{
	public Transform target;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	
	public float distanceMin = .5f;
	public float distanceMax = 15f;
	
	float x = 0.0f;
	float y = 0.0f;
	public bool freeze = false;
	
	// Use this for initialization
	void Start(){
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		//y = angles.x;
		y = 30;
	}
	
	void LateUpdate(){
		if(target && !freeze){
			if(Input.GetButton("Fire2") && !EventActivator.globalFreeze){
				x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
				y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			}
			
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			
			Quaternion rotation = Quaternion.Euler(y, x, 0);
			
			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);

			Vector3 negDistance = new Vector3(0.0f, 0.0f, - distance);
			Vector3 position = rotation * negDistance + target.position;
			
			transform.rotation = rotation;
			transform.position = position;
			//Vector3 trueTargetPosition = target.transform.position - new Vector3(0.0f,-targetHeight,0.0f);

			RaycastHit hit;
			if(Physics.Linecast(target.position, transform.position, out hit)){
				if(hit.transform.tag == "Wall"){
					float tempDistance = Vector3.Distance (target.transform.position, hit.point) - 0.28f;
					
					position = target.position - (rotation * Vector3.forward * tempDistance);
					transform.position = position;
				}
			}
		}
	}

	public void SetNewTarget(Transform t){
		target = t;
	}

	public static float ClampAngle(float angle, float min, float max){
		if(angle < -360F)
			angle += 360F;
		if(angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

	public void FreezeCamera(){
		freeze = true;
	}

	public void UnFreezeCamera(){
		freeze = false;
	}
}
