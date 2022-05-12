using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamRigController : MonoBehaviour{
		[SerializeField] [Required] private Transform rigRoot;
		[SerializeField] [MinValue(0.1f)] private float maxRigDistance = 0.25f;

		[SerializeField] [MinValue(5)] [MaxValue(20)]
		private int controlRigCount = 5;

		private List<Transform> _rigs;

		private DynamicBone _dynamicBone;

		private void Start(){
			_dynamicBone = GetComponent<DynamicBone>();
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
			var rigOffset = totalOffset / controlRigCount;
			for(var i = 0; i < controlRigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var addPosition = rigTransform.InverseTransformVector(rigOffset);
				var finalPosition = rigTransform.localPosition + addPosition;
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
				var rigCount = controlRigCount;
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
			controlRigCount = rigCount;
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

		public void ModifyInert(float amount){
			var currentInert = _dynamicBone.m_Inert;
			var nextInert = Mathf.Clamp01(currentInert + amount);
			_dynamicBone.m_Inert = nextInert;
			_dynamicBone.UpdateParameters();
		}
	}
}