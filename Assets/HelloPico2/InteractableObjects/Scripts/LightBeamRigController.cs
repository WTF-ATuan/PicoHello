using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamRigController : MonoBehaviour{
		[SerializeField] [Required] private Transform rigRoot;
		[SerializeField] [MinValue(0.1f)] private float maxRigDistance = 0.25f;
		[SerializeField] private int currentControlIndex = 5;
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
			for(var i = 0; i < currentControlIndex; i++){
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
		public void ModifyRigTotalLength(float totalLength){
			var totalOffset = rigRoot.forward * totalLength;
			var rigCount = Mathf.FloorToInt(totalOffset.z / maxRigDistance);
			var addedOffset = totalOffset / rigCount;
			for(var i = 0; i < rigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var finalPosition = rigTransform.InverseTransformVector(addedOffset);
				rigTransform.localPosition = finalPosition;
			}
		}

		public void SetPositionLenghtByPercent(float multiplyValue, float value){
			PostLenghtUpdatedEvent(0);
			for(var i = 0; i < currentControlIndex; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var targetPosition = rigTransform.InverseTransformVector(rigTransform.forward * multiplyValue);
				var lerpPosition = Vector3.Lerp(Vector3.zero, targetPosition, value);
				rigTransform.localPosition = lerpPosition;
				PostLenghtUpdatedEvent(1);
			}

			PostLenghtUpdatedEvent(2);
		}
	}
}