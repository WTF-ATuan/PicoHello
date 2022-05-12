using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(EventSetting))]

public class EventActivator : MonoBehaviour{
	[HideInInspector]
	public int runEvent = 0;
	public enum StartConditions{
		KeyTrigger,
		Collide,
		TriggerEnter,
		AutoStart,
		None //Call from another script
	}
	public StartConditions startCondition = StartConditions.KeyTrigger;
	public bool lookAtPlayer = false;
	public bool lockYAngle = false;
	private bool onLooking = false;
	public bool freezePlayerDuringEvent = false;

	[HideInInspector]
	public bool eventRunning = false;
	private Transform mainPlayer;

	public static bool globalFreeze = false;
	// Use this for initialization
	void Start(){
		if(startCondition == StartConditions.AutoStart){
			if(freezePlayerDuringEvent){
				FreezePlayer();
			}
			GetComponents<EventSetting>()[0].Activate();
		}
		if(startCondition == StartConditions.KeyTrigger){
			gameObject.tag = "TriggerEvent";
		}
	}

	void FreezePlayer(){
		globalFreeze = true;
		//If you have other Player Controller scripts and want to freeze character
		//or Disable controller during the event you can do your stuffs here.
		//For Example
		//FindPlayer();
		//mainPlayer.GetComponent<YourController>().enabled = false;
	}

	void Update(){
		if(onLooking){
			if(!mainPlayer){
				return;
			}
			Vector3 lookPos = mainPlayer.position - transform.position;
			if(lockYAngle){
				lookPos.y = 0;
			}
			if(lookPos != Vector3.zero){
				Quaternion rot = Quaternion.LookRotation(lookPos);
				transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 20);
			}
			
		}
	}

	public void ActivateTrigger(){
		if(startCondition == StartConditions.KeyTrigger){
			ActivateEvent();
		}
	}

	IEnumerator LookPlayer(){
		FindPlayer();
		if(mainPlayer){
			onLooking = true;
			yield return new WaitForSeconds(1);
			onLooking = false;
		}else{
			yield return new WaitForSeconds(0.01f);
		}
	}

	public void ActivateEvent(){
		if(runEvent > 0 || eventRunning){
			return;
		}
		if(lookAtPlayer){
			StartCoroutine(LookPlayer());
		}
		if(freezePlayerDuringEvent){
			FreezePlayer();
		}
		eventRunning = true;
		GetComponents<EventSetting>()[0].Activate();
	}

	public void ActivateEventId(int id){
		runEvent = id;
		eventRunning = true;
		if(freezePlayerDuringEvent){
			FreezePlayer();
		}
		GetComponents<EventSetting>()[runEvent].Activate();
	}

	void OnTriggerEnter(Collider other){
		if(other.tag == "Player" && startCondition == StartConditions.TriggerEnter){
			ActivateEvent();
		}
	}

	void OnCollisionEnter(Collision other){
		if(startCondition == StartConditions.Collide){
			ActivateEvent();
		}
	}

	void FindPlayer(){
		if(!mainPlayer){
			mainPlayer = GameObject.FindWithTag("Player").transform;
		}
	}

	public void EndEvent(){
		if(freezePlayerDuringEvent){
			globalFreeze = false;
			//If you have other Player Controller scripts and want to unfreeze character
			//or Enable controller during the event you can do your stuffs here.
			//For Example
			//FindPlayer();
			//mainPlayer.GetComponent<YourController>().enabled = true;
		}
		runEvent = 0;
		eventRunning = false;
	}
}
