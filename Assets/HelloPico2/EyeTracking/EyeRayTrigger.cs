using HelloPico2.InteractableObjects;
using Unity.XR.PXR;
using UnityEngine;

namespace HelloPico2.EyeTracking{
	public class EyeRayTrigger : MonoBehaviour{
		[SerializeField] private float rayCastMaxDistance = 100;
		public readonly HitTargetData TargetData = new HitTargetData();

		private void Update(){
			TrackEye();
		}

		private void TrackEye(){
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

		private void OpenPackage(string pkgName){
			using var jcPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			using var joActivity = jcPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			using var joPackageManager = joActivity.Call<AndroidJavaObject>("getPackageManager");
			using var joIntent = joPackageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", pkgName);
			if(null != joIntent) joActivity.Call("startActivity", joIntent);
		}
	}

	public class HitTargetData{
		public GameObject TargetObject;
		public Vector3 HitPoint;
	}
}