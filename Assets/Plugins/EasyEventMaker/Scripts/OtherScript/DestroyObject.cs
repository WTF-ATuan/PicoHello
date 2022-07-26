using UnityEngine;
using System.Collections;

public class DestroyObject : MonoBehaviour {
	public float DestoryTime = 5f;
	public float duration = 1.5f;

    private void Start()
    {
        Destroy(this.gameObject, DestoryTime);
    }
    public void DestroyObj(){
		Destroy (this.gameObject, duration);
	}
}