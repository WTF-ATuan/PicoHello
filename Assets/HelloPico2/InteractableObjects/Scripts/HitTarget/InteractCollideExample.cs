using DG.Tweening;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class InteractCollideExample : MonoBehaviour, IInteractCollide{
		public void OnCollide(InteractType type, Collider selfCollider){
			transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);
		}
	}
}