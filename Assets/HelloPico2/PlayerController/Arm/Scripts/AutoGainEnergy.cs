using System;
using System.Linq;
using Game.Project;
using HelloPico2.InputDevice.Scripts;
using Project;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm.Scripts{
	public class AutoGainEnergy : MonoBehaviour{
		[SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 6, 0);
		public bool active = true;
		public bool notGripGain = true;
		private ArmData _armData;
		private EnergyBallBehavior _energyBehavior;


		private ColdDownTimer _onInputTimer;
		private ColdDownTimer _normalTimer;

		private bool _isInvoking = false;

		private void Start(){
			_armData = GetComponent<ArmData>();
			_energyBehavior = GetComponent<EnergyBallBehavior>();
			EventBus.Subscribe<DeviceInputDetected>(OnInputDetected);
			_onInputTimer = new ColdDownTimer(curve.Evaluate(0));
			_normalTimer = new ColdDownTimer(0.4f);
		}

		private void Update(){
			if(!active || !notGripGain) return;
			if(_isInvoking) return;
			if(!_normalTimer.CanInvoke()) return;
			_normalTimer.ModifyDuring(0.4f);
			_normalTimer.Reset();
			CalculateEnergy();
		}


		private void OnInputDetected(DeviceInputDetected obj){
			if(Condition(obj)){
				return;
			}

			_isInvoking = true;
			var invokeDuring = curve.Evaluate(_armData.currentGripFunctionTimer);
			_onInputTimer.ModifyDuring(invokeDuring);
			_onInputTimer.Reset();
			CalculateEnergy();
			_isInvoking = false;
		}

		private bool Condition(DeviceInputDetected obj){
			return !obj.Selector.HandType.Equals(_armData.HandType) || !obj.IsGrip || !_onInputTimer.CanInvoke() ||
				   !_energyBehavior.isCurrentWeaponEnergyBall() || !active;
		}

		private void CalculateEnergy(){
			const float gainPercent = 0.02f;
			var energy = Mathf.Lerp(0, _armData.MaxEnergy, gainPercent);
			energy += _armData.Energy;
			if(_armData.Energy > _armData.MaxEnergy){
				return;
			}

			_energyBehavior.ChargeEnergyDirectlyWithoutVisualFeedback(energy);
		}

		public void Active(bool isActive){
			active = isActive;
		}
	}
}