using Project;
using UnityEngine;

namespace HelloPico2.Rating_System{
	public class RatingExecutor : MonoBehaviour{
		[SerializeField] private RatingSetting setting;

		private AngleCalculator _calculator;
		private BehaviorRating _behaviorRating;

		private int _ratingCount;

		private void Start(){
			EventBus.Subscribe<RatingInputRequested>(OnInputRequested);
		}

		private void OnInputRequested(RatingInputRequested obj){
			var origin = obj.Origin;
			var target = obj.Target;
			_ratingCount++;
			_calculator = new AngleCalculator(origin, target);
			_behaviorRating = new BehaviorRating(_calculator, setting);
		}
	}
}