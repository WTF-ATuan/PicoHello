using UnityEngine;
using System.Collections;

public class WeaponTrail : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		GetComponent<TrailRenderer>().enabled = transform.root.GetComponent<PlayerController>().onAttack;
	}
}
