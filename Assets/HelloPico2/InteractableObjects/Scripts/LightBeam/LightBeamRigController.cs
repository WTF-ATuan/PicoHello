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

		[SerializeField] [ProgressBar(1, 10)] private int lenghtLimit = 5;

		[SerializeField] [ProgressBar(0.1, 1)] [MaxValue(1)] [MinValue(0.1)]
		private float thickness = 1;

		[SerializeField] private AnimationCurve weightControlCurve;

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

		public void Floating(bool enable){
			if(enable){
				var rigTransform = transform;
				var targetPosition = rigTransform.position + rigTransform.right * 0.2f + rigTransform.up * 0.2f;
				rigTransform.DOMove(targetPosition, 1f).SetLoops(-1, LoopType.Yoyo);
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
				_dynamicBone.m_Root = rigRoot;
				_dynamicBone.UpdateRoot();
				_dynamicBone.UpdateParameters();
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

		[Button]
		public void ModifyControlRigLenght(float rigLenght){
			var addOffset = rigRoot.forward * rigLenght;
			if(IsLenghtLessThanZero(addOffset) || IsLenghtGreaterThanLimit(addOffset)) return;
			PostLenghtUpdatedEvent(0);
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

		public void SetPositionLenghtByPercent(float rigLenght, float value){
			PostLenghtUpdatedEvent(0);
			var targetPosition = rigRoot.forward * rigLenght;
			var lerpPosition = Vector3.Lerp(Vector3.zero, targetPosition, value);
			var rigOffset = lerpPosition / controlRigCount;
			for(var i = 0; i < controlRigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var addOffset = rigTransform.InverseTransformVector(rigOffset);
				rigTransform.localPosition = addOffset;
			}

			PostLenghtUpdatedEvent(2);
		}

		private bool IsLenghtLessThanZero(Vector3 addOffset){
			var lenghtUpdated = GetUpdateState();
			var localAddOffset = rigRoot.InverseTransformVector(addOffset);
			var totalOffset = lenghtUpdated.TotalOffset;
			return (totalOffset + localAddOffset).z < 0;
		}

		private bool IsLenghtGreaterThanLimit(Vector3 addOffset){
			var lenghtUpdated = GetUpdateState();
			var localAddOffset = rigRoot.InverseTransformVector(addOffset);
			var totalOffset = lenghtUpdated.TotalOffset;
			var limitOffset = rigRoot.forward * lenghtLimit;
			var localLimitOffset = rigRoot.InverseTransformVector(limitOffset);
			return (totalOffset + localAddOffset).z > localLimitOffset.z;
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

		public void ModifyBlendWeight(float amount){
			var currentBlendWeight = _dynamicBone.m_BlendWeight;
			var nextBlendWeight = Mathf.Clamp01(currentBlendWeight + amount);
			_dynamicBone.m_BlendWeight = nextBlendWeight;
			_dynamicBone.UpdateRoot();
			_dynamicBone.UpdateParameters();
		}
	}
}