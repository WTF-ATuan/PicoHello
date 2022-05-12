using UnityEngine;
using System.Collections;

public class SetParent : MonoBehaviour {
	public Transform target;

	void Start(){
		transform.parent = target;
	}

}
