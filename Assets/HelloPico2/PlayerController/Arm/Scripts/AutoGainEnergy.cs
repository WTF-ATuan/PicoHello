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
		private ColdDownTimer _notInputTimer;
        private float energyPercent;

        private void Start(){
			_armData = GetComponent<ArmData>();
			_energyBehavior = GetComponent<EnergyBallBehavior>();
			EventBus.Subscribe<DeviceInputDetected>(OnInputDetected);
			_onInputTimer = new ColdDownTimer(curve.Evaluate(0));
			_notInputTimer = new ColdDownTimer(0.25f);
		}

		private void OnInputDetected(DeviceInputDetected obj){
			if(!BasicCondition(obj))
				return;
			if(obj.IsGrip && _onInputTimer.CanInvoke()){
				GripGainEnergy();
			}

			if(!obj.IsGrip && _notInputTimer.CanInvoke() && notGripGain){
				energyPercent = _armData.Energy / _armData.MaxEnergy;
				if(energyPercent >= 0.75f) return;
				if(energyPercent < 0.35f){
					_notInputTimer.ModifyDuring(0.2f);
					_notInputTimer.Reset();
					GainEnergyDirectly(10f);
				}
				else{
					_notInputTimer.ModifyDuring(0.4f);
					_notInputTimer.Reset();
					GainEnergyDirectly(15f);
				}
			}
		}

		private void GripGainEnergy(){
			var invokeDuring = curve.Evaluate(_armData.currentGripFunctionTimer);
			_onInputTimer.ModifyDuring(invokeDuring);
			_onInputTimer.Reset();
			GainEnergyByPercent(0.02f);
		}

		private bool BasicCondition(DeviceInputDetected obj){
			return obj.Selector.HandType.Equals(_armData.HandType) && _energyBehavior.isCurrentWeaponEnergyBall() &&
				   active;
		}

		private void GainEnergyByPercent(float amount){
			var energy = Mathf.Lerp(0, _armData.MaxEnergy, amount);
			energy += _armData.Energy;
			if(_armData.Energy > _armData.MaxEnergy){
				return;
			}

			_energyBehavior.ChargeEnergyDirectly(energy);
		}

		private void GainEnergyDirectly(float amount){
			var energy = amount + _armData.Energy;
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