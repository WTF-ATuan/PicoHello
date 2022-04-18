using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Game.Project;

public class BallGrabScript : MonoBehaviour{
	public float missTime = 0.5f;
	public float maxTime = 2.0f;
	public float missSpeed = 0.2f;
	public float defaultSpeed = 1.0f;
	public float maxSpeed = 10.0f;
	float speed;
	public float destroyTime=3f;
	public Animator anim;
	public AudioClip[] hitAudio;
	AudioSource source;
	float ballType = 0;
	bool isShoot = false;
	bool isSel = false;
	bool isLine = false;
	Vector3 localPos;
	float countTime = 0.0f;
	Transform prefabObj;

	private Rigidbody _rigidbody;
	private XRGrabInteractable _grabInteractable;
	[SerializeField] private int additionForce = 1;
	[SerializeField] private float chargeTime = 2f;
	private ColdDownTimer _timer;

	// Start is called before the first frame update
	private void Start(){
		anim = GetComponent<Animator>();
		source = GetComponent<AudioSource>();
		_rigidbody = GetComponent<Rigidbody>();
		Invoke("startDestroy", Random.Range(3,10));
		prefabObj=gameObject.transform.GetChild(0);

		_grabInteractable = GetComponent<XRGrabInteractable>();
		_timer = new ColdDownTimer(chargeTime);
		_grabInteractable.selectEntered.AddListener(OnSelectEntered);
		_grabInteractable.selectExited.AddListener(OnSelectExited);
		_grabInteractable.throwVelocityScale *= additionForce;

	}

	// Update is called once per frame
	private void FixedUpdate(){
		
		if (isSel == true){
			countTime += Time.deltaTime;
            if (countTime < missTime)
            {
				prefabObj.transform.localScale = Vector3.one * 0.2f;

            }
			else if (missTime < countTime && countTime < maxTime)
            {
				prefabObj.transform.localScale = Vector3.one * (countTime/ maxTime);
				anim.speed = countTime/ maxTime;
			}
			else if(countTime> maxTime)
            {
				prefabObj.transform.localScale = Vector3.one;
				anim.speed = 3.0f;
			}
		}

		if(!isShoot) return;

		if(speed == missSpeed){
			transform.Translate(Vector3.up * Time.deltaTime*0.1f, Space.Self);
			transform.localScale = new Vector3(transform.localScale.x * 0.1f, transform.localScale.y * 0.1f,
				transform.localScale.z * 0.1f);
			destroyObj(0.3f);
		}

		if (!isLine) return; 
		
		_rigidbody.isKinematic = true;
		transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
		destroyObj(3);


	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "target" || other.tag == "ground")
		{
			destroyObj(0);
		}

	}
	
	private void destroyObj(float destime)
    {
		Destroy(gameObject, destime);
	}
	private void startDestroy()
	{
		Destroy(gameObject);
	}
	public void ballSelect(){
		isSel = true;
		GetComponent<SphereCollider>().radius = 0.05f;
	}

	private void OnSelectEntered(SelectEnterEventArgs obj)
	{
		CancelInvoke("startDestroy");
		_timer.Reset();
		isSel = true;
	}
	private void OnSelectExited(SelectExitEventArgs obj)
	{
		var velocityMagnitude = _rigidbody.velocity.magnitude;
		
		if(velocityMagnitude > 1.0f)
        {
			_grabInteractable.throwOnDetach = _timer.CanInvoke();
		}
        else
        {
			isLine = true;
		}
		shootBall(countTime);
		//countTime = 0;
	}

	private void shootBall(float currentTime){
	localPos = transform.localPosition;
	isSel = false;
		
	if(currentTime < missTime){
		source.PlayOneShot(hitAudio[0], 1);
		anim.SetBool("isClose", true);
			anim.speed = 2.0f;
		speed = missSpeed;
	}
	else if(missTime < currentTime && currentTime < chargeTime)
		{
		anim.speed = 0;
		speed = defaultSpeed;
		source.PlayOneShot(hitAudio[1], 1);
	}
	else if(currentTime > chargeTime)
		{
		anim.speed = 0;
		speed = maxSpeed;
		source.PlayOneShot(hitAudio[2], 1);
		ballType = 1;
	}

	isShoot = true;
	}
}
