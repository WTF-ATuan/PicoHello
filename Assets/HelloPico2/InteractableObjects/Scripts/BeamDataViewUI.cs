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
		[SerializeField] private Text speedLimitText;
		[SerializeField] private Text delaySoftTimeText;
		[SerializeField] private LightBeamRigController controller;
		[SerializeField] private LightBeamEventHandler eventHandler;


		private void Start(){
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			var isSameObject = obj.IsSameObject(controller.gameObject);
			var isPadClick = obj.IsPadClick;
			if(isPadClick){
				gameObject.SetActive(!gameObject.activeSelf);
			}

			if(!isSameObject) return;
			var lengthUpdated = controller.GetUpdateState();
			var selectorSpeed = obj.Selector.Speed;
			var blendWeight = lengthUpdated.BlendWeight;
			beamSoftSlider.value = blendWeight;
			beamSoftPercentText.text = blendWeight.ToString(CultureInfo.InvariantCulture);
			inputDeviceSpeedText.text = selectorSpeed.ToString(CultureInfo.InvariantCulture);
			speedLimitText.text = eventHandler.speedLimit.ToString(CultureInfo.InvariantCulture);
			delaySoftTimeText.text = eventHandler.returnDuring.ToString(CultureInfo.InvariantCulture);
		}

		public void ModifySpeedLimit(bool isAdded){
			if(isAdded){
				eventHandler.speedLimit += 0.1f;
			}
			else{
				eventHandler.speedLimit -= 0.1f;
			}
		}

		public void ModifySoftDelay(bool isAdded){
			if(isAdded){
				eventHandler.returnDuring += 0.1f;
			}
			else{
				eventHandler.returnDuring -= 0.1f;
			}
		}
	}
}