using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class SpawnTargetTracker : MonoBehaviour{
		[TitleGroup("Target Setting")] [EnumToggleButtons]
		public TargetType targetType;

		[ShowIf("IsTransformType")] [TitleGroup("Target Setting")]
		public Transform targetTransform;

		[ShowIf("IsVectorType")] [TitleGroup("Target Setting")]
		public Vector3 targetVector;

		[TitleGroup("Duration Setting")] public float during = 2;
		[TitleGroup("Duration Setting")] public AnimationCurve movingCurve = AnimationCurve.Linear(0, 0, 1, 1);

		[TitleGroup("Complete Action")] [EnumToggleButtons]
		public CompleteAction completeAction;

		[TitleGroup("Complete Action")] [ShowIf("IsDelayAction")]
		public float delayTime = 1f;

		[TitleGroup("Debug")] public bool debugLine;
		[TitleGroup("Debug")] public Color debugColor = Color.green;

		private ISpawner _spawner;

		private void OnEnable(){
			_spawner = GetComponent<ISpawner>();
			if(_spawner == null) throw new Exception($"Can,t Get Spawner in {gameObject}");
			_spawner.OnSpawn += OnSpawn;
		}

		private void OnDisable(){
			_spawner.OnSpawn -= OnSpawn;
		}

		private void OnSpawn(GameObject obj){
			var objTransform = obj.transform;
			var targetPosition = GetTargetPosition();
			objTransform.DOLookAt(targetPosition, 0.1f);
			objTransform.DOMove(targetPosition, during)
					.SetEase(movingCurve)
					.OnComplete(() => OnCompleteMoving(obj));
		}

		private void OnCompleteMoving(GameObject obj){
			switch(completeAction){
				case CompleteAction.None:
					break;
				case CompleteAction.Destroy:
					Destroy(obj, delayTime);
					break;
				case CompleteAction.Inactive:
					obj.transform.DOScale(Vector3.zero, delayTime)
							.OnComplete(() => obj.SetActive(false));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private Vector3 GetTargetPosition(){
			switch(targetType){
				case TargetType.MainCamera:
					var main = Camera.main;
					if(!main) throw new Exception("Main Camera is null");
					var cameraPosition = main.transform.position;
					return cameraPosition;
				case TargetType.Transform:
					var position = targetTransform.position;
					return position;
				case TargetType.Vector:
					return targetVector;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnDrawGizmosSelected(){
			if(!debugLine){
				return;
			}

			var targetPosition = GetTargetPosition();
			Gizmos.color = debugColor;
			Gizmos.DrawLine(transform.position, targetPosition);
		}


		private bool IsTransformType() => targetType == TargetType.Transform;
		private bool IsVectorType() => targetType == TargetType.Vector;
		private bool IsDelayAction() =>
				completeAction == CompleteAction.Destroy || completeAction == CompleteAction.Inactive;
	}

	public enum TargetType{
		MainCamera,
		Transform,
		Vector
	}

	public enum CompleteAction{
		None,
		Destroy,
		Inactive,
	}
}