using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.EyeTracking{
	public class SimpleMover : MonoBehaviour{
		[SerializeField] private Transform controlObject;
		[SerializeField] private float during;

		[Button]
		public void Move(Vector3 targetPosition){
			controlObject.DOMove(targetPosition, during)
					.SetEase(Ease.Linear);
		}
	}
}