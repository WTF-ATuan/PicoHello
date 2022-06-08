using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace HelloPico2.InteractableObjects{
	public class LightBeamRigController : MonoBehaviour{
		[SerializeField] [Required] private Transform rigRoot;

		[SerializeField] [ProgressBar(1, 25)] [MaxValue(50)]
		private int controlRigCount = 5;

		[FormerlySerializedAs("maxMinLenghtLimit")] [SerializeField] [MinMaxSlider(0, 20)]
		public Vector2Int minMaxLenghtLimit;

		[SerializeField] [ReadOnly] private float currentLengthLimit = 5;

		[SerializeField] [ProgressBar(0.1, 1)] [MaxValue(1)] [MinValue(0.1)]
		private float thickness = 1;

		private List<Transform> _rigs;

		private DynamicBone _dynamicBone;

		private CapsuleCollider _capsuleCollider;

		[SerializeField] private AnimationCurve lengthChangedCurve;


		public void SetLengthLimit(float percentage){
			if(percentage > 1){
				throw new Exception($"inputValue is greater than 1 {percentage}");
			}

			var lerpValue = Mathf.Lerp(minMaxLenghtLimit.x, minMaxLenghtLimit.y, percentage);
			currentLengthLimit = lerpValue;
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

		[Button]
		public void ModifyControlRigLength(float rigLength){
			var totalAddOffset = rigRoot.forward * rigLength;
			if(IsLengthLessThanZero(totalAddOffset)){
				SetRigTotalLength(0);
				return;
			}

			if(IsLengthGreaterThanLimit(totalAddOffset) && rigLength > 0) return;
			var rigOffset = totalAddOffset / controlRigCount;
			var rigLocalOffset = rigRoot.InverseTransformVector(rigOffset);
			SetRigLength(controlRigCount, rigLocalOffset, true);
		}

		public void SetPositionLengthByPercent(float rigLength, float value){
			PostLengthUpdatedEvent(0);
			var targetPosition = rigRoot.forward * rigLength;
			var lerpPosition = Vector3.Lerp(Vector3.zero, targetPosition, value);
			var rigOffset = lerpPosition / controlRigCount;
			for(var i = 0; i < controlRigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var addOffset = rigTransform.InverseTransformVector(rigOffset);
				rigTransform.localPosition = addOffset;
			}

			PostLengthUpdatedEvent(2);
		}

		[Button]
		public void SetRigTotalLength(float offsetMultiplier){
			var totalOffset = rigRoot.forward * offsetMultiplier;
			var rigCount = controlRigCount;
			var rigOffset = totalOffset / rigCount;
			var rigLocalOffset = rigRoot.InverseTransformVector(rigOffset);
			SetRigLength(rigCount, rigLocalOffset);
		}

		public LightBeamLengthUpdated GetUpdateState(){
			var lengthUpdated = new LightBeamLengthUpdated();
			var totalOffset = _rigs.Aggregate(Vector3.zero, (current, rig) => current + rig.localPosition);
			var singleOffset = _rigs.First().localPosition;
			lengthUpdated.TotalLength = totalOffset.magnitude;
			lengthUpdated.SingleLength = singleOffset.magnitude;
			lengthUpdated.TotalOffset = totalOffset;
			lengthUpdated.UpdateState = 0;
			lengthUpdated.CurrentLengthLimit = currentLengthLimit;
			lengthUpdated.MaxLengthLimit = minMaxLenghtLimit.y;
			lengthUpdated.MinLengthLimit = minMaxLenghtLimit.x;
			lengthUpdated.BlendWeight = _dynamicBone.m_BlendWeight;
			return lengthUpdated;
		}

		public float GetLengthModifiedCurveSpeed(){
			var lightBeamLengthUpdated = GetUpdateState();
			var totalLength = lightBeamLengthUpdated.TotalLength;
			var lengthLimit = lightBeamLengthUpdated.CurrentLengthLimit;
			var progress = totalLength / lengthLimit;
			var curveValue = lengthChangedCurve.Evaluate(progress);
			return curveValue;
		}

		public void Init(){
			_dynamicBone = GetComponent<DynamicBone>();
			_capsuleCollider = GetComponent<CapsuleCollider>();
			_rigs = rigRoot.GetComponentsInChildren<Transform>().ToList();
			_rigs.RemoveAt(0);
		}

		private void ModifyThickness(float percent){
			var localScale = transform.localScale;
			thickness = percent;
			localScale.x = Mathf.Lerp(1, 10, thickness);
			localScale.y = Mathf.Lerp(1, 10, thickness);
			transform.localScale = localScale;
		}

		private void Start(){
			Init();
		}

		private void PostLengthUpdatedEvent(int updateState){
			var lengthUpdated = GetUpdateState();
			lengthUpdated.UpdateState = updateState;
			if(updateState == 0) SetDynamicBoneActive(false);

			if(updateState == 2){
				UpdateBoneCollider(lengthUpdated);
				UpdateBeamThickness(lengthUpdated);
				SetDynamicBoneActive(true);
			}
		}

		private void SetDynamicBoneActive(bool enable){
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

		private void UpdateBeamThickness(LightBeamLengthUpdated lengthUpdated){
			var totalOffset = lengthUpdated.TotalOffset;
			var totalLength = rigRoot.TransformVector(totalOffset).magnitude;
			var finalPercent = 1 - totalLength * 0.1f + 0.1f;
			ModifyThickness(finalPercent);
		}

		private void UpdateBoneCollider(LightBeamLengthUpdated lengthUpdated){
			var totalOffset = lengthUpdated.TotalOffset;
			var totalLength = lengthUpdated.TotalLength;
			var centerOfCollider = totalOffset / 2;
			_capsuleCollider.center = centerOfCollider;
			_capsuleCollider.height = totalLength;
		}

		private void OnTriggerEnter(Collider other){
			var collides = other.gameObject.GetComponents<IInteractCollide>();
			collides.ForEach(c => c?.OnCollide(InteractType.Beam));
		}

		private bool IsLengthLessThanZero(Vector3 addOffset){
			var lengthUpdated = GetUpdateState();
			var localAddOffset = rigRoot.InverseTransformVector(addOffset);
			var totalOffset = lengthUpdated.TotalOffset;
			return (totalOffset + localAddOffset).z < 0;
		}

		private bool IsLengthGreaterThanLimit(Vector3 addOffset){
			var lengthUpdated = GetUpdateState();
			var localAddOffset = rigRoot.InverseTransformVector(addOffset);
			var totalOffset = lengthUpdated.TotalOffset;
			var limitOffset = rigRoot.forward * currentLengthLimit;
			var localLimitOffset = rigRoot.InverseTransformVector(limitOffset);
			return (totalOffset + localAddOffset).z > localLimitOffset.z;
		}

		private void SetRigLength(int rigCount, Vector3 rigOffset, bool isAdd = false){
			PostLengthUpdatedEvent(0);
			for(var i = 0; i < rigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				if(isAdd){
					rigTransform.localPosition += rigOffset;
				}
				else{
					rigTransform.localPosition = rigOffset;
				}

				PostLengthUpdatedEvent(1);
			}

			PostLengthUpdatedEvent(2);
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