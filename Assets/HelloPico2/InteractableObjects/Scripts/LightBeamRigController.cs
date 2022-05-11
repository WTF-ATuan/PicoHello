using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamRigController : MonoBehaviour{
		[SerializeField] [Required] private Transform rigRoot;
		[SerializeField] [MinValue(0.1f)] private float maxRigDistance = 0.25f;
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
			var totalOffset = rigRoot.forward * rigLenght;
			var rigCount = GetRigCountByTotalLength(totalOffset);
			if(rigCount == 0) SetRigTotalLength(0);
			var offset = totalOffset / rigCount;
			for(var i = 0; i < rigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var modifiedPosition = rigTransform.InverseTransformVector(offset);
				var finalPosition = rigTransform.localPosition + modifiedPosition;
				rigTransform.localPosition = finalPosition;
				PostLenghtUpdatedEvent(1);
			}

			PostLenghtUpdatedEvent(2);
		}

		[Button]
		public void SetRigTotalLength(float offsetMultiplier){
			var totalOffset = rigRoot.forward * offsetMultiplier;

			if(offsetMultiplier > 0){
				var rigCount = Mathf.FloorToInt(totalOffset.z / maxRigDistance);
				var addedOffset = totalOffset / rigCount;
				SetRigLength(rigCount, addedOffset);
			}
			else{
				var rigCount = GetRigCountByTotalLength(Vector3.zero);
				var decreaseOffset = totalOffset / rigCount;
				SetRigLength(rigCount, decreaseOffset);
			}
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
		}

		[Button]
		private int GetRigCountByTotalLength(Vector3 addedOffset){
			var maxLength = _rigs.Max(x => x.position.magnitude);
			var maxOffset = _rigs.Find(x => x.position.magnitude >= maxLength).position;
			var targetOffset = maxOffset + addedOffset;
			var rigCount = Mathf.Abs(Mathf.FloorToInt(targetOffset.z / maxRigDistance));
			return rigCount;
		}

		public void SetPositionLenghtByPercent(float multiplyValue, float value){
			PostLenghtUpdatedEvent(0);
			var totalOffset = rigRoot.forward * multiplyValue;
			var rigCount = GetRigCountByTotalLength(totalOffset);
			if(rigCount == 0) SetRigTotalLength(0);
			var offset = totalOffset / rigCount;
			for(var i = 0; i < rigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var targetPosition = rigTransform.InverseTransformVector(offset);
				var lerpPosition = Vector3.Lerp(Vector3.zero, targetPosition, value);
				rigTransform.localPosition = lerpPosition;
				PostLenghtUpdatedEvent(1);
			}

			PostLenghtUpdatedEvent(2);
		}
	}
}