using UnityEngine;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;

public class DestroyObject : MonoBehaviour {
	public float DestoryTime = 5f;
	public float duration = 1.5f;
	
	[BoxGroup("NEW")]
	public float fadeoutDuring = 1f;
	[BoxGroup("NEW")]
	public bool selfDestroy = false;

	private Transform[] _childTransform;
    private void Start()
    {
		Invoke(nameof(DoFadeOut) , DestoryTime - fadeoutDuring);
        Destroy(this.gameObject, DestoryTime);
    }
    public void DestroyObj(){
		Destroy (this.gameObject, duration);
	}

	public void DoFadeOut(){
		if(selfDestroy){
			transform.DOScale(Vector3.zero, fadeoutDuring).SetEase(Ease.Linear);
		}
		else{
			_childTransform = GetComponentsInChildren<Transform>();
			foreach(var child in _childTransform){
				child.DOScale(Vector3.zero, fadeoutDuring).SetEase(Ease.Linear);
			}
		}
	}
}