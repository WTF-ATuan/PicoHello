using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.EyeTracking{
	public class SimpleMover : MonoBehaviour{
		[SerializeField] private Transform controlObject;
		[SerializeField] private float during;
		[SerializeField] private bool ignoreZ = true;
		[SerializeField] private AnimationCurve movingCurve = AnimationCurve.Linear(0, 0, 1, 1);


		[Button]
		public void Move(Vector3 targetPosition){
			if(ignoreZ){
				targetPosition.z = controlObject.position.z;
				controlObject.DOMove(targetPosition, during)
						.SetEase(movingCurve);
			}
			else{
				controlObject.DOMove(targetPosition, during)
						.SetEase(movingCurve);
			}
		}
	}
}