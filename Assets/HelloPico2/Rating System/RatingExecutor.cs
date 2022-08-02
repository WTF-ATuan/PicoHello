using Project;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.Rating_System{
	public class RatingExecutor : MonoBehaviour{
		[SerializeField] private RatingSetting setting;

		private AngleCalculator _calculator;
		private BehaviorRating _behaviorRating;

		private int _ratingCount;
		private float _ratingPoint;

		private void Start(){
			EventBus.Subscribe<RatingInputRequested>(OnInputRequested);
		}

		[Button]
		private void TestEvent(){
			var origin = GameObject.Find("Origin").transform;
			var target = GameObject.Find("Target").transform;
			EventBus.Post(new RatingInputRequested(origin, target));
		}

		private void OnInputRequested(RatingInputRequested obj){
			var origin = obj.Origin;
			var target = obj.Target;
			_ratingCount++;
			_calculator = new AngleCalculator(origin, target);
			_behaviorRating = new BehaviorRating(_calculator, setting);
			CalculateHitRate();
		}

		private void CalculateHitRate(){
			var offsetRating = _behaviorRating.GetOffsetRating();
			_ratingPoint += offsetRating;
			var hitRate = _ratingPoint / _ratingCount;
			Debug.Log($"{hitRate} = ({_ratingPoint} / {_ratingCount})");
		}
	}
}