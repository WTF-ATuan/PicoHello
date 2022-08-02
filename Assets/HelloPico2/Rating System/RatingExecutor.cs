using System;
using Project;
using UnityEngine;

namespace HelloPico2.Rating_System{
	public class RatingExecutor : MonoBehaviour{
		[SerializeField] private RatingSetting setting;

		private BehaviorRating _behaviorRating;
		private void Start(){
			EventBus.Subscribe<RatingInputRequested>(OnInputRequested);
		}

		private void OnInputRequested(RatingInputRequested obj){
			
		}
	}
}