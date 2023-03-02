using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.EyeTracking{
	public class SimpleMover : MonoBehaviour{
		[SerializeField] private Transform controlObject;
		[SerializeField] private float during;
		[SerializeField] private AnimationCurve movingCurve = AnimationCurve.Linear(0, 0, 1, 1);
		[SerializeField] private float maxTargetDistance = 10;
		public bool useOffset;
		[ShowIf("useOffset")] public Vector3 offset = Vector3.one;
		public bool rotate = false;
		[ShowIf("rotate")] public float wavyNess = 0.3f;
		[ShowIf("rotate")] public float bufferDistance = 30f;


		public bool debug;
		private Vector3 _previousTargetPoint;
		private List<Vector3> _currentSlerpPoints;

		public void Move(Vector3 targetPosition){
			if(useOffset) targetPosition += offset;
			SmoothMovement(targetPosition);
		}

		private void SmoothMovement(Vector3 targetPosition){
			targetPosition = Vector3.ClampMagnitude(targetPosition, maxTargetDistance);
			controlObject.DOMove(targetPosition, during)
					.SetEase(movingCurve);
			if(!rotate) return;
			targetPosition.y -= wavyNess * targetPosition.y;
			var dir = targetPosition - controlObject.position;
			controlObject.up = Vector3.Lerp(Vector3.up, dir, dir.magnitude / bufferDistance);
		}

		private void CurveMovement(Vector3 targetPosition){
			var position = controlObject.position;
			var center = (position + targetPosition) * 0.5f;
			var distance = Vector3.Distance(position, targetPosition);
			if((targetPosition - _previousTargetPoint).magnitude > 0.1f){
				_currentSlerpPoints =
						EvaluateSlerpPoints(position, targetPosition, center, (int)Mathf.Clamp(distance * 10, 10, 50));
				_currentSlerpPoints.RemoveAt(0);
			}

			var closestPoint = GetClosestPoint(position, Mathf.Infinity);
			controlObject.DOMove(closestPoint, during).SetEase(movingCurve);
			if((position - closestPoint).magnitude < 1f){
				if(_currentSlerpPoints.Count > 1){
					_currentSlerpPoints.Remove(closestPoint);
				}
			}

			_previousTargetPoint = targetPosition;
		}

		private Vector3 GetClosestPoint(Vector3 position, float closestDistanceSqr){
			var closePoint = Vector3.zero;
			foreach(var points in _currentSlerpPoints){
				var directionToTarget = points - position;
				var dSqrToTarget = directionToTarget.sqrMagnitude;
				if(!(dSqrToTarget < closestDistanceSqr)) continue;
				closestDistanceSqr = dSqrToTarget;
				closePoint = points;
			}

			return closePoint;
		}

		private List<Vector3> EvaluateSlerpPoints(Vector3 start, Vector3 end, Vector3 center, int count = 10){
			var startRelativeCenter = start - center;
			var endRelativeCenter = end - center;

			var f = 1f / count;
			var points = new List<Vector3>();
			for(var i = 0f; i < 1 + f; i += f){
				points.Add(Vector3.Slerp(startRelativeCenter, endRelativeCenter, i) + center);
			}

			return points;
		}

		private void OnDrawGizmos(){
			if(!Application.isPlaying || !debug) return;
			var center = (controlObject.position + _previousTargetPoint) * 0.5f;
			var slerpPoints = EvaluateSlerpPoints(controlObject.position, _previousTargetPoint,
				center);
			foreach(var point in slerpPoints){
				Gizmos.DrawSphere(point, 0.1f);
			}

			Gizmos.color = Color.red;
			Gizmos.DrawSphere(controlObject.position, 0.2f);
		}
	}
}