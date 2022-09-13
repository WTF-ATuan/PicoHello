using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.LevelTool{
	public class CurveMover : MonoBehaviour{
		[OnValueChanged("CalculateTarget")] [SerializeField] private Vector3 targetOffset;
		[SerializeField] [ReadOnly] private Vector3 targetPosition;
		[SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
		[SerializeField] private float during = 2f;


		private void OnEnable(){
			CalculateTarget();
			transform.DOMove(targetPosition, during).SetEase(curve);
		}


		private void CalculateTarget(){
			targetPosition = transform.position + targetOffset;
		}
	}
}