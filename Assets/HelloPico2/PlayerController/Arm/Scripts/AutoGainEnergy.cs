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

		private bool _isGaining = false;

		private void Start(){
			_armData = GetComponent<ArmData>();
			_energyBehavior = GetComponent<EnergyBallBehavior>();
			EventBus.Subscribe<DeviceInputDetected>(OnInputDetected);
			_onInputTimer = new ColdDownTimer(curve.Evaluate(0));
			_normalTimer = new ColdDownTimer(0.25f);
		}

		private void Update(){
			if(NotGripCondition()) return;
			var energyPercent = _armData.Energy / _armData.MaxEnergy;
			if(energyPercent >= 0.7f) return;
			if(energyPercent < 0.5f){
				_normalTimer.ModifyDuring(0.6f);
				_normalTimer.Reset();
				CalculateEnergy(0.4f);
			}
			else{
				_normalTimer.ModifyDuring(0.25f);
				_normalTimer.Reset();
				CalculateEnergy(0.5f);
			}
		}

		private bool NotGripCondition(){
			return !active || !notGripGain || _isGaining || !_normalTimer.CanInvoke() ||
				   !_energyBehavior.isCurrentWeaponEnergyBall();
		}


		private void OnInputDetected(DeviceInputDetected obj){
			if(Condition(obj)){
				return;
			}

			_isGaining = true;
			var invokeDuring = curve.Evaluate(_armData.currentGripFunctionTimer);
			_onInputTimer.ModifyDuring(invokeDuring);
			_onInputTimer.Reset();
			CalculateEnergy(0.02f);
			_isGaining = false;
		}

		private bool Condition(DeviceInputDetected obj){
			return !obj.Selector.HandType.Equals(_armData.HandType) || !obj.IsGrip || !_onInputTimer.CanInvoke() ||
				   !_energyBehavior.isCurrentWeaponEnergyBall() || !active;
		}

		private void CalculateEnergy(float amount){
			var energy = Mathf.Lerp(0, _armData.MaxEnergy, amount);
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