using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamRigController : MonoBehaviour{
		[SerializeField] [Required] private Transform rigRoot;

		[SerializeField] [ProgressBar(1, 25)] [MaxValue(50)]
		private int controlRigCount = 5;

		private List<Transform> _rigs;

		private DynamicBone _dynamicBone;

		private void Start(){
			_dynamicBone = GetComponent<DynamicBone>();
			_rigs = rigRoot.GetComponentsInChildren<Transform>().ToList();
			_rigs.RemoveAt(0);
			_rigs[controlRigCount + 1].gameObject.SetActive(false);
		}

		private void PostLenghtUpdatedEvent(int updateState){
			var lenghtUpdated = new LightBeamLenghtUpdated();
			var totalOffset = _rigs.Aggregate(Vector3.zero, (current, rig) => current + rig.localPosition);
			var singleOffset = _rigs.First().localPosition;
			lenghtUpdated.TotalLenght = totalOffset.magnitude;
			lenghtUpdated.SingleLenght = singleOffset.magnitude;
			lenghtUpdated.TotalOffset = totalOffset;
			lenghtUpdated.UpdateState = updateState;
			if(updateState == 0) EnableDynamicBone(false);

			if(updateState == 2){
				UpdateBoneCollider(lenghtUpdated);
				EnableDynamicBone(true);
			}
		}

		private void EnableDynamicBone(bool enable){
			if(!enable){
				_dynamicBone.m_Root = null;
				_dynamicBone.UpdateRoot();
			}
			else{
				StopAllCoroutines();
				StartCoroutine(DelayChangeRoot());
			}
		}

		private void UpdateBoneCollider(LightBeamLenghtUpdated lenghtUpdated){
			var totalOffset = lenghtUpdated.TotalOffset;
			var centerOfCollider = totalOffset / 2;
			var capsuleCollider = GetComponent<CapsuleCollider>();
			capsuleCollider.center = centerOfCollider;
		}

		private IEnumerator DelayChangeRoot(){
			yield return new WaitForFixedUpdate();
			_dynamicBone.m_Root = rigRoot;
			_dynamicBone.UpdateRoot();
			_dynamicBone.UpdateParameters();
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
			var rigCount = controlRigCount;
			var rigOffset = totalOffset / rigCount;
			SetRigLength(rigCount, rigOffset);
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