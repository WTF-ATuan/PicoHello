using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]

public class BulletStatus : MonoBehaviour {
	public int damage = 5;
	public float duration = 1.5f;
	public GameObject hitEffect;
	public bool flinchTarget = false;
	public bool penetrate = false;
	public string shooterTag = "Player";

	[HideInInspector]
	public int totalDamage = 0;

	// Use this for initialization
	void Start(){
		Destroy(gameObject , duration);
		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Collider>().isTrigger = true;
	}

	public void Setting(int atk , string tag){
		shooterTag = tag;
		totalDamage = (damage + atk);
	}

	void OnTriggerEnter(Collider other){  	
		//When Player Shoot at Enemy
		if(shooterTag == "Player" && other.tag == "Enemy"){	  
			other.GetComponent<Status>().OnDamage(totalDamage);

			if(hitEffect){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			if(flinchTarget){
				other.GetComponent<Status>().Flinch();
			}
			if(!penetrate){
				Destroy(gameObject);
			}
			//When Enemy Shoot at Player
		}else if(shooterTag == "Enemy" && other.tag == "Player"){  	
			other.GetComponent<Status>().OnDamage(totalDamage);
			
			if(hitEffect){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			if(flinchTarget){
				other.GetComponent<Status>().Flinch();
			}
			if(!penetrate){
				Destroy(gameObject);
			}
		}else if(other.gameObject.tag == "Wall") {
			if(hitEffect){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			if(!penetrate){
				Destroy(gameObject);
			}
		}
	}
}
