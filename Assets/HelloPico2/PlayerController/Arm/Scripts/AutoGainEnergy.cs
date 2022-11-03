using Game.Project;
using HelloPico2.InputDevice.Scripts;
using Project;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm.Scripts{
	public class AutoGainEnergy : MonoBehaviour{
		[SerializeField] private AnimationCurve curve = AnimationCurve.Linear(1, 1, 0, 0);

		private ArmData _armData;
		private EnergyBallBehavior _energyBehavior;


		private float _timer;
		private ColdDownTimer _frameTimer;


		private void Start(){
			_armData = GetComponent<ArmData>();
			_energyBehavior = GetComponent<EnergyBallBehavior>();
			EventBus.Subscribe<DeviceInputDetected>(OnInputDetected);
			_frameTimer = new ColdDownTimer(0.2f);
		}

		private void OnInputDetected(DeviceInputDetected obj){
			if(Condition(obj)){
				_timer = 0;
				return;
			}

			_timer += Time.deltaTime;
			CalculateEnergy();
			_frameTimer.Reset();
		}

		private bool Condition(DeviceInputDetected obj){
			return !obj.Selector.HandType.Equals(_armData.HandType) || !obj.IsGrip || !_frameTimer.CanInvoke();
		}

		private void CalculateEnergy(){
			var lerpTime = Mathf.Lerp(0, 6, _timer);
			var timeValue = curve.Evaluate(lerpTime);
			var energy = Mathf.Lerp(0, _armData.MaxEnergy, timeValue);
			energy += _armData.Energy;
			if(energy >= _armData.MaxEnergy){
				return;
			}

			_energyBehavior.ChargeEnergyDirectly(energy);
		}
	}
}