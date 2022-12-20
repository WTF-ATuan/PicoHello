using System.Collections.Generic;
using DG.Tweening;
using HelloPico2.InteractableObjects;
using UltEvents;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR;

namespace HelloPico2.EyeTracking{
	public class EyeRayTrigger : MonoBehaviour{
		[SerializeField] private float rayCastMaxDistance = 100;
		[SerializeField] private Transform signObject;
		[SerializeField] private LayerMask detectedLayer;

		public UltEvent<Vector3> eyeHitEvent;

		private void Update(){
			if(CheckEyeDevice()){
				TrackEye();
			}
			else{
				TrackHead();
			}

			InputDevices.GetDeviceAtXRNode(XRNode.LeftHand)
					.TryGetFeatureValue(CommonUsages.menuButton, out var isLMenuButton);
			InputDevices.GetDeviceAtXRNode(XRNode.RightHand)
					.TryGetFeatureValue(CommonUsages.menuButton, out var isRMenuButton);
			if(isLMenuButton || isRMenuButton) CalibrationEyeTracker();
		}

		private void TrackHead(){
			if(!Camera.main) return;
			var cameraTransform = Camera.main.transform;
			var originPoint = cameraTransform.position;
			var direction = cameraTransform.forward;
			var ray = new Ray(originPoint, direction);
			if(!Physics.Raycast(ray, out var hit, rayCastMaxDistance, detectedLayer)) return;
			signObject.position = hit.point;
			Trigger(hit.collider, hit.point);
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
			if(!Physics.Raycast(ray, out var hit, rayCastMaxDistance, detectedLayer)) return;
			signObject.position = hit.point;
			Trigger(hit.collider, hit.point);
		}

		private bool CheckEyeDevice(){
			var devices = new List<UnityEngine.XR.InputDevice>();
			InputDevices.GetDevicesWithCharacteristics(
				InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.HeadMounted, devices);
			return devices.Count != 0 && devices[0].isValid;
		}

		private void Trigger(Collider hitTarget, Vector3 position){
			var collide = hitTarget.GetComponent<IInteractCollide>();
			collide?.OnCollide(InteractType.Eye, hitTarget);
			eyeHitEvent.Invoke(position);
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