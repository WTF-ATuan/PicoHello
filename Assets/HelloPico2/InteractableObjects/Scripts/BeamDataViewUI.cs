using System;
using System.Globalization;
using HelloPico2.InputDevice.Scripts;
using Project;
using UnityEngine;
using UnityEngine.UI;

namespace HelloPico2.InteractableObjects{
	public class BeamDataViewUI : MonoBehaviour{
		[SerializeField] private Slider beamSoftSlider;
		[SerializeField] private Text beamSoftPercentText;
		[SerializeField] private Text inputDeviceSpeedText;
		[SerializeField] private LightBeamRigController controller;

		private void Start(){
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			var isSameObject = obj.IsSameObject(controller.gameObject);
			if(!isSameObject) return;
			var lengthUpdated = controller.GetUpdateState();
			var selectorSpeed = obj.Selector.Speed;
			var blendWeight = lengthUpdated.BlendWeight;
			beamSoftSlider.value = blendWeight;
			beamSoftPercentText.text = blendWeight.ToString(CultureInfo.InvariantCulture);
			inputDeviceSpeedText.text = selectorSpeed.ToString(CultureInfo.InvariantCulture);
		}
	}
}