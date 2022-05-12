using UnityEngine;
using System.Collections;

public class HealingItem : MonoBehaviour {
	public int amount = 20;

	void OnTriggerEnter(Collider other){  	
		if(other.tag == "Player" && other.GetComponent<Status>()){	 
			other.GetComponent<Status>().Healing(amount);
			Destroy(gameObject);
		}
	}
}
