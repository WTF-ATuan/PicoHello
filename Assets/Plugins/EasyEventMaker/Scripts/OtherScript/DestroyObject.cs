using UnityEngine;
using System.Collections;

public class DestroyObject : MonoBehaviour {
	
	public float duration = 1.5f;
	
	void Start(){
		Destroy (gameObject, duration);
	}
}