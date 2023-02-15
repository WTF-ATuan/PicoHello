using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.EyeTracking{
	public class SimpleMover : MonoBehaviour{
		[SerializeField] private Transform controlObject;
		[SerializeField] private float during;
		[SerializeField] private bool ignoreZ = true;
		[SerializeField] private AnimationCurve movingCurve = AnimationCurve.Linear(0, 0, 1, 1);
		public bool useOffset;
		[ShowIf("useOffset")] public Vector3 offset = Vector3.one;


		public void Move(Vector3 targetPosition){
			if(ignoreZ){
				targetPosition.z = controlObject.position.z;
				if(useOffset) targetPosition += offset;
				controlObject.DOMove(targetPosition, during)
						.SetEase(movingCurve);
			}
			else{
				if(useOffset) targetPosition += offset;
				controlObject.DOMove(targetPosition, during)
						.SetEase(movingCurve);
			}
		}
		[Button]
		private void Test(Transform target){
			Move(target.position);
		}
	}
}