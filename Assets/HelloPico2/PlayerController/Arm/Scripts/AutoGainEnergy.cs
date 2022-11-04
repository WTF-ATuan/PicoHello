using Game.Project;
using HelloPico2.InputDevice.Scripts;
using Project;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm.Scripts{
	public class AutoGainEnergy : MonoBehaviour{
		[SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 6, 0);

		private ArmData _armData;
		private EnergyBallBehavior _energyBehavior;


		private ColdDownTimer _frameTimer;


		private void Start(){
			_armData = GetComponent<ArmData>();
			_energyBehavior = GetComponent<EnergyBallBehavior>();
			EventBus.Subscribe<DeviceInputDetected>(OnInputDetected);
			_frameTimer = new ColdDownTimer(curve.Evaluate(0));
		}

		private void OnInputDetected(DeviceInputDetected obj){
			if(Condition(obj)){
				return;
			}

			var invokeDuring = curve.Evaluate(_armData.currentGripFunctionTimer);
			print(invokeDuring);
			_frameTimer.ModifyDuring(invokeDuring);
			_frameTimer.Reset();
			CalculateEnergy();
		}

		private bool Condition(DeviceInputDetected obj){
			return !obj.Selector.HandType.Equals(_armData.HandType) || !obj.IsGrip || !_frameTimer.CanInvoke() ||
				   !_energyBehavior.isCurrentWeaponEnergyBall();
		}

		private void CalculateEnergy(){
			const float gainPercent = 0.02f;
			var energy = Mathf.Lerp(0, _armData.MaxEnergy, gainPercent);
			energy += _armData.Energy;
			if(_armData.Energy > _armData.MaxEnergy){
				return;
			}

			_energyBehavior.ChargeEnergyDirectly(energy);
		}
	}
}