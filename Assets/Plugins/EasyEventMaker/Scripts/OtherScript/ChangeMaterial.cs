using UnityEngine;
using System.Collections;

public class ChangeMaterial : MonoBehaviour {
	public Material swapMaterial;

	public void SwapMaterial(){
		GetComponent<Renderer>().material = swapMaterial;
	}

}
