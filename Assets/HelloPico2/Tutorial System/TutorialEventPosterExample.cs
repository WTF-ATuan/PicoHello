using HelloPico2.InputDevice.Scripts;
using Project;
using UnityEngine;

namespace HelloPico2.TutorialSystem{
	public class TutorialEventPosterExample : MonoBehaviour{
		private void Start(){
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			var selectableObject = obj.Selector.SelectableObject;
			SandBehaviorRequest(selectableObject);
		}

		private void SandBehaviorRequest(GameObject obj){ }
	}
}