using UnityEngine;

namespace HelloPico2.Rating_System{
	public class AngleCalculator{
		private readonly Transform _origin;
		private readonly Transform _target;

		public AngleCalculator(Transform origin, Transform target){
			_origin = origin;
			_target = target;
		}

		public float GetAngleOfTarget(){
			var targetPosition = _target.position;
			var originPosition = _origin.position;
			var originForward = _origin.forward;
			var direction = targetPosition - originPosition;
			return Vector3.Angle(originForward, direction);
		}

		public float GetOffsetOfTarget(){
			var targetPosition = _target.position;
			var originPosition = _origin.position;
			var originForward = _origin.forward;
			var distance = Vector3.Distance(targetPosition, originPosition);
			var trianglePoint = originPosition + originForward * distance;
			return Vector3.Distance(trianglePoint, targetPosition);
		}
	}
}