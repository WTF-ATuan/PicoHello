using System;
using Game.Project;
using HelloPico2.InputDevice.Scripts;
using Project;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm.Scripts{
	public class AutoGainEnergy : MonoBehaviour{
		[SerializeField] private AnimationCurve curve = AnimationCurve.Linear(1, 1, 0, 0);
		private const float UpdateFrameTime = 0.2f;

		private ArmData _armData;
		private EnergyBallBehavior _energyBehavior;


		private float _timer;
		private ColdDownTimer _frameTimer;


		private void Start(){
			_armData = GetComponent<ArmData>();
			_energyBehavior = GetComponent<EnergyBallBehavior>();
			EventBus.Subscribe<DeviceInputDetected>(OnInputDetected);
			_frameTimer = new ColdDownTimer(UpdateFrameTime);
		}

		private void OnInputDetected(DeviceInputDetected obj){
			if(!_frameTimer.CanInvoke()) return;
			if(!obj.IsGrip){
				_timer = 0;
				return;
			}

			_timer += Time.deltaTime;
			CalculateEnergy();
			_frameTimer.Reset();
		}

		private void CalculateEnergy(){
			var lerpTime = Mathf.Lerp(0, 6, _timer);
			var timeValue = curve.Evaluate(lerpTime);
			var energy = Mathf.Lerp(0, _armData.MaxEnergy, timeValue);
			energy += _armData.Energy;
			_energyBehavior.ChargeEnergyDirectly(energy);
		}
	}
}