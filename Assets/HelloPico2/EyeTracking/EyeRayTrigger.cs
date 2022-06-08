using DG.Tweening;
using HelloPico2.InteractableObjects;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR;

namespace HelloPico2.EyeTracking{
	public class EyeRayTrigger : MonoBehaviour{
		[SerializeField] private float rayCastMaxDistance = 100;
		[SerializeField] private Transform signObject;


		private void Update(){
			TrackEye();
			InputDevices.GetDeviceAtXRNode(XRNode.LeftHand)
					.TryGetFeatureValue(CommonUsages.menuButton, out var isLMenuButton);
			InputDevices.GetDeviceAtXRNode(XRNode.RightHand)
					.TryGetFeatureValue(CommonUsages.menuButton, out var isRMenuButton);
			if(isLMenuButton || isRMenuButton) CalibrationEyeTracker();
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
			signObject.DOMove(hit.point, 1 / Time.deltaTime).SetEase(Ease.Linear);
			var collide = hit.collider.GetComponent<IInteractCollide>();
			collide?.OnCollide(InteractType.Eye, null);
		}

		public void CalibrationEyeTracker(){
			OpenPackage("com.tobii.usercalibration.pico");
			OpenPackage("com.tobii.usercalibration.neo3");
		}

		private void OpenPackage(string pkgName){
			using var jcPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			using var joActivity = jcPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			using var joPackageManager = joActivity.Call<AndroidJavaObject>("getPackageManager");
			using var joIntent = joPackageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", pkgName);
			if(null != joIntent) joActivity.Call("startActivity", joIntent);
		}
	}
}