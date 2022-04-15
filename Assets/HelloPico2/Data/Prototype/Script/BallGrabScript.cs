using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGrabScript : MonoBehaviour{
	public float missTime = 0.5f;
	public float missSpeed = 0.2f;
	public float defaultSpeed = 1.0f;
	public float maxSpeed = 10.0f;
	float speed;
	public float destroyTime=3f;
	public Animator anim;
	public AudioClip[] hitAudio;
	AudioSource source;
	bool isShoot = false;
	bool isSel = false;
	Vector3 localPos;
	float countTime = 0.0f;

	private Rigidbody _rigidbody;

	// Start is called before the first frame update
	private void Start(){
		anim = GetComponent<Animator>();
		source = GetComponent<AudioSource>();
		_rigidbody = GetComponent<Rigidbody>();
		Invoke("destroyObj", 3);
	}

	// Update is called once per frame
	private void FixedUpdate(){
		if(isSel == true){
			countTime += Time.deltaTime;
		}

		if(!isShoot) return;

		if(speed == missSpeed){
			transform.Translate(Vector3.up * Time.deltaTime * speed, Space.Self);
			transform.localScale = new Vector3(transform.localScale.x * 0.3f, transform.localScale.y * 0.3f,
				transform.localScale.z * 0.3f);
			Destroy(this.gameObject, 0.8f);
		}
		else{
			var velocityMagnitude = _rigidbody.velocity.magnitude;
			if(velocityMagnitude < 20f){
				transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
			}
            else
            {
				_rigidbody.isKinematic = false;
				_rigidbody.useGravity = true;
            }
		}
	}

	private void OnTriggerEnter(Collider other){
		if(other.tag == "target" || other.tag == "ground"){
			Destroy(this.gameObject);
		}
		if(other.tag == "Player")
        {
			CancelInvoke("destroyObj");
		}
	}
	
	private void destroyObj()
    {
		Destroy(gameObject);
	}

	public void ballSelect(){
		isSel = true;
		GetComponent<SphereCollider>().radius = 0.05f;
	}

	public void shootBall(){
		localPos = transform.localPosition;
		isSel = false;

		if(countTime < missTime){
			source.PlayOneShot(hitAudio[0], 1);
			anim.SetBool("isClose", true);
			speed = missSpeed;
		}
		else if(0.3f < countTime && countTime < 2.0f){
			anim.speed = 0;
			speed = defaultSpeed;
			source.PlayOneShot(hitAudio[1], 1);
		}
		else if(countTime > 2.0f){
			anim.speed = 0;
			speed = maxSpeed;
			source.PlayOneShot(hitAudio[2], 1);
		}

		isShoot = true;
	}
}