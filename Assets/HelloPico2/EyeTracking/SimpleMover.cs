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
		public bool targeting;
		[ShowIf("targeting")] public AnimationCurve targetingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

		private Vector3 _previousTargetPos;
		private float _targetingTimer;

		public void Move(Vector3 targetPosition){
			SmoothMovement(GetTraceTarget(targetPosition));
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

		private Vector3 GetTraceTarget(Vector3 oriTargetPos){
			if(!useOffset) return oriTargetPos;
			var distance = Vector3.Distance(oriTargetPos, _previousTargetPos);
			if(distance < 0.1f){
				_targetingTimer += Time.deltaTime;
				var lerpingValue = 1 - targetingCurve.Evaluate(_targetingTimer); //TargetCurve => 0 => 0.5f
				var lerpingPoint = Vector3.Lerp(Vector3.zero, offset, lerpingValue);
				_previousTargetPos = oriTargetPos;
				oriTargetPos += lerpingPoint;
			}
			else{
				_targetingTimer = 0;
				_previousTargetPos = oriTargetPos;
				oriTargetPos += offset;
			}

			return oriTargetPos;
		}
	}
}