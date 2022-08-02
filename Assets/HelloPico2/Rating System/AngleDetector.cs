using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.Rating_System{
	public class AngleDetector : MonoBehaviour{
		[SerializeField] private Transform origin;
		[SerializeField] private Transform target;

		[SerializeField] private bool showGizmos = true;

		[SerializeField] [ReadOnly] private float angle;
		[SerializeField] [ReadOnly] private float offset;

		private Vector3 _trianglePoint;

		private void FixedUpdate(){
			var targetPosition = target.position;
			var originPosition = origin.position;
			var originForward = origin.forward;
			var direction = targetPosition - originPosition;
			angle = Vector3.Angle(originForward, direction);
			var distance = Vector3.Distance(targetPosition, originPosition);
			_trianglePoint = originPosition + originForward * distance;
			offset = Vector3.Distance(_trianglePoint, targetPosition);
		}

		public float GetAngleOfTarget(){
			var targetPosition = target.position;
			var originPosition = origin.position;
			var originForward = origin.forward;
			var direction = targetPosition - originPosition;
			return Vector3.Angle(originForward, direction);
		}

		public float GetOffsetOfTarget(){
			var targetPosition = target.position;
			var originPosition = origin.position;
			var originForward = origin.forward;
			var distance = Vector3.Distance(targetPosition, originPosition);
			var trianglePoint = originPosition + originForward * distance;
			return Vector3.Distance(trianglePoint, targetPosition);
		}

		private void OnDrawGizmos(){
			if(!showGizmos) return;
			Gizmos.color = Color.red;
			var originPosition = origin.position;
			var targetPosition = target.position;
			Gizmos.DrawLine(originPosition, targetPosition);
			Gizmos.DrawLine(originPosition, _trianglePoint);
			Gizmos.DrawLine(_trianglePoint, targetPosition);
			Gizmos.DrawWireCube(_trianglePoint, Vector3.one);
		}
	}
}