using UnityEngine;

namespace HelloPico2.Rating_System{
	public class RatingInputRequested{
		public Transform Origin{ get; }
		public Transform Target{ get; }

		public RatingInputRequested(Transform origin, Transform target){
			Origin = origin;
			Target = target;
		}
	}
}