using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Project;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamRigController : MonoBehaviour{
		[SerializeField] [Required] private Transform rigRoot;

		[SerializeField] [ProgressBar(1, 25)] [MaxValue(50)]
		private int controlRigCount = 5;

		[SerializeField] [ProgressBar(0.1, 1)] [MaxValue(1)] [MinValue(0.1)]
		private float thickness = 1;

		private List<Transform> _rigs;

		private DynamicBone _dynamicBone;

		private CapsuleCollider _capsuleCollider;


		private void OnValidate(){
			var isPlaying = Application.isPlaying;
			if(!isPlaying) return;
			var localScale = transform.localScale;
			localScale.x = Mathf.Lerp(1, 10, thickness);
			localScale.y = Mathf.Lerp(1, 10, thickness);
			transform.localScale = localScale;
		}
		[Button]
		public void Floating(bool enable){
			if(enable){
				var rigTransform = transform;
				var targetPosition = rigTransform.position + rigTransform.up * 0.5f;
				rigTransform.DOMove(targetPosition, 0.5f).SetLoops(-1, LoopType.Yoyo);
			}
			else{
				transform.DOKill(true);
			}
		}

		private void ModifyThickness(float percent){
			var localScale = transform.localScale;
			thickness = percent;
			localScale.x = Mathf.Lerp(1, 10, thickness);
			localScale.y = Mathf.Lerp(1, 10, thickness);
			transform.localScale = localScale;
		}

		private void Start(){
			_dynamicBone = GetComponent<DynamicBone>();
			_capsuleCollider = GetComponent<CapsuleCollider>();
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
				UpdateBeamThickness(lenghtUpdated);
				EnableDynamicBone(true);
			}
		}

		private LightBeamLenghtUpdated GetUpdateState(){
			var lenghtUpdated = new LightBeamLenghtUpdated();
			var totalOffset = _rigs.Aggregate(Vector3.zero, (current, rig) => current + rig.localPosition);
			var singleOffset = _rigs.First().localPosition;
			lenghtUpdated.TotalLenght = totalOffset.magnitude;
			lenghtUpdated.SingleLenght = singleOffset.magnitude;
			lenghtUpdated.TotalOffset = totalOffset;
			lenghtUpdated.UpdateState = 0;
			return lenghtUpdated;
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

		private void UpdateBeamThickness(LightBeamLenghtUpdated lenghtUpdated){
			var totalOffset = lenghtUpdated.TotalOffset;
			var totalLenght = rigRoot.TransformVector(totalOffset).magnitude;
			var finalPercent = 1 - totalLenght * 0.1f + 0.1f;
			ModifyThickness(finalPercent);
		}

		private void UpdateBoneCollider(LightBeamLenghtUpdated lenghtUpdated){
			var totalOffset = lenghtUpdated.TotalOffset;
			var totalLenght = lenghtUpdated.TotalLenght;
			var centerOfCollider = totalOffset / 2;
			_capsuleCollider.center = centerOfCollider;
			_capsuleCollider.height = totalLenght;
		}

		private void OnTriggerEnter(Collider other){
			var collides = other.gameObject.GetComponents<IBeamCollide>();
			collides.ForEach(c => c?.OnCollide());
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
			var addOffset = rigRoot.forward * rigLenght;
			if(IsLenghtLessThanZero(addOffset)) return;
			var rigOffset = addOffset / controlRigCount;
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

		private bool IsLenghtLessThanZero(Vector3 addOffset){
			var lenghtUpdated = GetUpdateState();
			var currentOffset = rigRoot.TransformVector(lenghtUpdated.TotalOffset);
			if((currentOffset + addOffset).IsLesserOrEqual(Vector3.zero)) return true;

			return false;
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