using UnityEngine;
using System.Collections;

public class ReloadScene : MonoBehaviour {
	public float delay = 5;
	// Use this for initialization
	void Start(){
		StartCoroutine(DelayLoad());
	}
	
	IEnumerator DelayLoad(){
		yield return new WaitForSeconds(delay);
		Application.LoadLevel(Application.loadedLevel);
	}
}
