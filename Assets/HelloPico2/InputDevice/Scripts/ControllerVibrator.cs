using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.Serialization;

namespace HelloPico2.InputDevice.Scripts{
	public class ControllerVibrator : MonoBehaviour{
		public HandType handType;

		public void VibrateByClip(AudioClip clip){
			var handIndex = handType switch{
				HandType.Left => 1,
				HandType.Right => 2,
			};

			PXR_Input.StartVibrateBySharem(clip, handIndex, 0);
		}
	}
}