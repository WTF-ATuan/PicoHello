using HelloPico2.InputDevice.Scripts;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InputDevice.PicoDeviceUse{
	public class HapticExample : MonoBehaviour{
		private ISelector _selector;

		private void Start(){
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			_selector = obj.Selector;
		}

		[Button]
		public void Haptic(){
			_selector.SetHaptic(10, 1);
		}
	}
}