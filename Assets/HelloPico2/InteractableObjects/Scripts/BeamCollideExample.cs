using DG.Tweening;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class BeamCollideExample : MonoBehaviour, IBeamCollide{
		public void OnCollide(){
			transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);
		}
	}
}