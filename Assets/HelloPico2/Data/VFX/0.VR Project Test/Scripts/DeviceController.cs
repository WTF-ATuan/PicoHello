using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Data.VFX._0.VR_Project_Test.Scripts{
	public class DeviceController : MonoBehaviour{
		[SerializeField] private ParticleAdjuster adjuster;
		private XRController _controller;
		private UnityEngine.XR.InputDevice _inputDevice;

		private int _rgbIndex;

		private void Start(){
			_controller = GetComponent<XRController>();
			_inputDevice = _controller.inputDevice;
		}

		private void Update(){
			var controllerNode = _controller.controllerNode;
			if(controllerNode == XRNode.RightHand){
				ChangeVelocity();
			}

			if(controllerNode == XRNode.LeftHand){
				ChangeShaderColor();
				ChangeRgbIndex();
			}
		}

		private void ChangeVelocity(){
			_inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var touchpadAxis);
			if(touchpadAxis.magnitude < 0.1f) return;
			var axisX = touchpadAxis.x;
			adjuster.ModifyVelocity(axisX * 0.1f);
		}

		private bool _primaryButtonDownFlag;
		private bool _secondaryButtonDownFlag;

		private void ChangeRgbIndex(){
			_inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out var primaryButtonValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out var secondaryButtonValue);
			if(primaryButtonValue && !_primaryButtonDownFlag){
				_rgbIndex++;
				_primaryButtonDownFlag = true;
			}
			else{
				_primaryButtonDownFlag = false;
			}

			if(secondaryButtonValue && !_secondaryButtonDownFlag){
				_rgbIndex++;
				_secondaryButtonDownFlag = true;
			}
			else{
				_secondaryButtonDownFlag = false;
			}
		}

		private void ChangeShaderColor(){
			_inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var touchpadAxis);
			if(touchpadAxis.magnitude < 0.1f) return;
			var axisX = touchpadAxis.x * 0.01f;
			switch(_rgbIndex){
				case 0:
					adjuster.ModifyColor(axisX, 0, 0);
					break;
				case 1:
					adjuster.ModifyColor(0, axisX, 0);
					break;
				case 2:
					adjuster.ModifyColor(0, 0, axisX);
					break;
			}
		}
	}
}