using HelloPico2.InputDevice.Scripts;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InputDevice.PicoDeviceUse{
	public class HapticExample : MonoBehaviour{
		private ISelector _rightSelector;
		private ISelector _leftSelector;

		private void Start(){
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			//判定是否是右手 是的話就抓取 
			if(obj.Selector.HandType == HandType.Right){
				_rightSelector = obj.Selector;
			}

			if(obj.Selector.HandType == HandType.Left){
				_leftSelector = obj.Selector;
			}
		}

		[Button]
		public void Haptic(){
			_rightSelector.SetHaptic(10, 1);
			_leftSelector.SetHaptic(20, 3);
		}
	}
}