using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamRigController : MonoBehaviour{
		[SerializeField] [Required] private Transform rigRoot;
		[SerializeField] [MinValue(0.1f)] private float maxRigDistance = 0.25f;
		[SerializeField] private int controlRigCount = 5;
		private List<Transform> _rigs;

		private void Start(){
			_rigs = rigRoot.GetComponentsInChildren<Transform>().ToList();
			_rigs.RemoveAt(0);
		}

		private void PostLenghtUpdatedEvent(int updateState){
			var lenghtUpdated = new LightBeamLenghtUpdated();
			var totalOffset = _rigs.Aggregate(Vector3.zero, (current, rig) => current + rig.localPosition);
			var singleOffset = _rigs.First().localPosition;
			lenghtUpdated.TotalLenght = totalOffset.magnitude;
			lenghtUpdated.SingleLenght = singleOffset.magnitude;
			lenghtUpdated.UpdateState = updateState;
		}

		[Button]
		public void ModifyControlRigLenght(float rigLenght){
			PostLenghtUpdatedEvent(0);
			for(var i = 0; i < controlRigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var addPosition = rigTransform.InverseTransformVector(rigTransform.forward * rigLenght);
				var finalPosition = rigTransform.localPosition + addPosition;
				rigTransform.localPosition = finalPosition;
				PostLenghtUpdatedEvent(1);
			}

			PostLenghtUpdatedEvent(2);
		}

		[Button]
		public void SetRigTotalLength(float offsetMultiplier){
			var totalOffset = rigRoot.forward * offsetMultiplier;
			var rigCount = GetRigCountByTotalLength(totalOffset);
			var offset = totalOffset / rigCount;
			SetRigLength(rigCount, offset);
		}

		private void SetRigLength(int rigCount, Vector3 rigOffset){
			PostLenghtUpdatedEvent(0);
			for(var i = 0; i < rigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var finalPosition = rigTransform.InverseTransformVector(rigOffset);
				rigTransform.localPosition = finalPosition;
				PostLenghtUpdatedEvent(1);
			}

			PostLenghtUpdatedEvent(2);
			controlRigCount = rigCount;
		}

		private int GetRigCountByTotalLength(Vector3 addedOffset){
			var totalOffset = _rigs.Aggregate(Vector3.zero, (current, rig) => current + rig.position);
			var targetOffset = totalOffset + addedOffset;
			var rigCount = Mathf.Abs(Mathf.FloorToInt(targetOffset.z / maxRigDistance));
			return rigCount;
		}

		public void SetPositionLenghtByPercent(float multiplyValue, float value){
			PostLenghtUpdatedEvent(0);
			for(var i = 0; i < controlRigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var targetPosition = rigTransform.InverseTransformVector(rigTransform.forward * multiplyValue);
				var lerpPosition = Vector3.Lerp(Vector3.zero, targetPosition, value);
				rigTransform.localPosition = lerpPosition;
				PostLenghtUpdatedEvent(1);
			}

			PostLenghtUpdatedEvent(2);
		}

		#if UNITY_EDITOR
		[Button]
		public void ResetRigPosition(){
			var rigs = rigRoot.GetComponentsInChildren<Transform>().ToList();
			rigs.RemoveAt(0);
			foreach(var rig in rigs){
				rig.localPosition = Vector3.zero;
			}
		}

		#endif
	}
}