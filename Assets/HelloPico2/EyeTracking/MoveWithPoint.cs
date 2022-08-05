using System;
using DG.Tweening;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.EyeTracking{
	public class MoveWithPoint : MonoBehaviour{
		[SerializeField] private Vector3 limitOffset;
		[SerializeField] private float during;

		[SerializeField] [ReadOnly] private Vector3 moveLimitNeg, moveLimitPos;
		private Vector3 _startPosition;

		private void Start(){
			_startPosition = transform.position;
			moveLimitNeg = _startPosition - limitOffset;
			moveLimitPos = _startPosition + limitOffset;
		}

		[Button]
		public void Move(Vector3 movePoint){
			var direction = movePoint.normalized;
			var targetPosition = transform.position + direction;
			var targetX = targetPosition.x;
			var targetZ = targetPosition.z;
			if(targetX > moveLimitPos.x || targetX < moveLimitNeg.x) return;
			if(targetZ > moveLimitPos.z || targetZ < moveLimitNeg.z) return;

			transform.DOMove(targetPosition, during)
					.SetEase(Ease.Linear);
		}
	}
}