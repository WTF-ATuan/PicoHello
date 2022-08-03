using Project;
using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;

namespace HelloPico2.Rating_System{
	public class RatingExecutor : MonoBehaviour{
		[SerializeField] private RatingSetting setting;

		private AngleCalculator _calculator;
		private BehaviorRating _behaviorRating;

		private int _ratingCount;
		private float _ratingPoint;
		private float _hitRate;

		[ReadOnly] public string playerState = "Non Rating";

		[SerializeField] private UltEvent masterLevel;
		[SerializeField] private UltEvent middenLevel;
		[SerializeField] private UltEvent weekLevel;

		private void Start(){
			EventBus.Subscribe<RatingInputRequested>(OnInputRequested);
		}

		private void OnInputRequested(RatingInputRequested obj){
			var origin = obj.Origin;
			var target = obj.Target;
			_ratingCount++;
			_calculator = new AngleCalculator(origin, target);
			_behaviorRating = new BehaviorRating(_calculator, setting);
			CalculateHitRate();
			CalculateRatingLevel();
		}

		private void CalculateHitRate(){
			var offsetRating = _behaviorRating.GetOffsetRating();
			_ratingPoint += offsetRating;
			_hitRate = _ratingPoint / _ratingCount;
			//Debug.Log($"{_hitRate} = ({_ratingPoint} / {_ratingCount})");
		}

		private void CalculateRatingLevel(){
			var ignoreCount = setting.minimumIgnoreCount;
			if(_ratingCount < ignoreCount) return;
			var master = _hitRate > setting.masterLevel;
			var midden = _hitRate > setting.middenLevel;
			var week = _hitRate > setting.weekLevel;
			if(master){
				ChangePlayerState("Master");
				return;
			}

			if(midden){
				ChangePlayerState("Midden");
				return;
			}

			if(week){
				ChangePlayerState("Week");
				return;
			}
		}

		private void ChangePlayerState(string state){
			if(playerState.Equals(state)) return;
			Debug.Log($"State Changed to {state} from {playerState}");
			playerState = state;
			switch(playerState){
				case "Master":
					masterLevel.Invoke();
					break;
				case "Midden":
					middenLevel.Invoke();
					break;
				case "Week":
					weekLevel.Invoke();
					break;
			}
		}
	}
}