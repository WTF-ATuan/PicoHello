using HelloPico2.InteractableObjects;
using Unity.XR.PXR;
using UnityEngine;

namespace HelloPico2.EyeTracking{
	public class EyeRayTrigger : MonoBehaviour{
		[SerializeField] private float rayCastMaxDistance = 100;
		public readonly HitTargetData TargetData = new HitTargetData();

		private void Update(){
			if(!Camera.main) return;
			var cameraTransform = Camera.main.transform;
			var matrix4X4 = Matrix4x4.TRS(cameraTransform.position, cameraTransform.rotation, Vector3.one);
			PXR_EyeTracking.GetCombineEyeGazePoint(out var origin);
			PXR_EyeTracking.GetCombineEyeGazeVector(out var direction);
			var originOffset = matrix4X4.MultiplyPoint(origin);
			var directionOffset = matrix4X4.MultiplyVector(direction);
			var ray = new Ray(originOffset, directionOffset);
			if(!Physics.Raycast(ray, out var hit, rayCastMaxDistance)) return;
			TargetData.TargetObject = hit.collider.gameObject;
			TargetData.HitPoint = hit.point;
			var collide = hit.collider.GetComponent<IInteractCollide>();
			collide?.OnCollide(InteractType.Eye);
		}
	}

	public class HitTargetData{
		public GameObject TargetObject;
		public Vector3 HitPoint;
	}
}