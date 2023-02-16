using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.EyeTracking{
	public class SimpleMover : MonoBehaviour{
		[SerializeField] private Transform controlObject;
		[SerializeField] private float during;
		[SerializeField] private AnimationCurve movingCurve = AnimationCurve.Linear(0, 0, 1, 1);
		public bool useOffset;
		[ShowIf("useOffset")] public Vector3 offset = Vector3.one;


		public void Move(Vector3 targetPosition){
			if(useOffset) targetPosition += offset;
			var position = controlObject.position;
			var distance = Vector3.Distance(targetPosition, position);
			if(distance > 3){
				var yOffset = new Vector3(0, Mathf.Sin(Time.time * 1.5f) * 0.1f, 0);
				controlObject.position = Vector3.Lerp(position + yOffset, targetPosition + yOffset,
					Time.fixedDeltaTime);
			}else
			{
				var yOffset = new Vector3(0, Mathf.Sin(Time.time * 3f) * 0.01f, 0);
				controlObject.position = Vector3.Lerp(position + yOffset, targetPosition + yOffset,
					Time.fixedDeltaTime);
			}


		}


		private IEnumerable<Vector3> EvaluateSlerpPoints(Vector3 start, Vector3 end, float centerOffset){
			var centerPivot = (start + end) * 0.5f;
			centerPivot -= new Vector3(0, -centerOffset);
			var startRelativeCenter = start - centerPivot;
			var endRelativeCenter = end - centerPivot;
			var f = 1f / 10;
			for(var i = 0; i < 1 + f; i++){
				yield return Vector3.Slerp(startRelativeCenter, endRelativeCenter, i) + centerPivot;
			}
		}
	}
}