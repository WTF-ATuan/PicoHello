using Game.Project;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace HelloPico2.EyeTracking{
	public class RandomTargetGenerator : MonoBehaviour{
		public UnityEvent<Vector3> invokeTarget;
		public float pointBetween = 5f;
		public float during = 0.3f;
		private ColdDownTimer _timer;
		
		private void Start(){
			_timer = new ColdDownTimer(during);
		}

		private void Update(){
			if(!_timer.CanInvoke()) return;
			invokeTarget?.Invoke(RandomPos());
			_timer.Reset();
		}

		private Vector3 RandomPos(){
			var randomPos = new Vector3(Random.Range(-pointBetween, pointBetween), Random.Range(0, 1),
				Random.Range(-pointBetween, pointBetween));
			return randomPos;
		}
	}
}