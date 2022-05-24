using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamRigController : MonoBehaviour{
		[SerializeField] [Required] private Transform rigRoot;

		[SerializeField] [ProgressBar(1, 25)] [MaxValue(50)]
		private int controlRigCount = 5;

		[SerializeField] [ProgressBar(1, 10)] private int lengthLimit = 5;

		[SerializeField] [ProgressBar(0.1, 1)] [MaxValue(1)] [MinValue(0.1)]
		private float thickness = 1;

		[SerializeField] private AnimationCurve weightControlCurve;

		private List<Transform> _rigs;

		private DynamicBone _dynamicBone;

		private CapsuleCollider _capsuleCollider;

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

			if(IsLengthGreaterThanLimit(totalAddOffset)) return;
			var rigOffset = totalAddOffset / controlRigCount;
			var rigLocalOffset = rigRoot.InverseTransformVector(rigOffset);
			var rigFinalOffset = _rigs.First().localPosition + rigLocalOffset;
			SetRigLength(controlRigCount, rigFinalOffset);
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

		private LightBeamLengthUpdated GetUpdateState(){
			var lengthUpdated = new LightBeamLengthUpdated();
			var totalOffset = _rigs.Aggregate(Vector3.zero, (current, rig) => current + rig.localPosition);
			var singleOffset = _rigs.First().localPosition;
			lengthUpdated.TotalLength = totalOffset.magnitude;
			lengthUpdated.SingleLength = singleOffset.magnitude;
			lengthUpdated.TotalOffset = totalOffset;
			lengthUpdated.UpdateState = 0;
			return lengthUpdated;
		}

		private void SetDynamicBoneActive(bool enable){
			if(!enable){
				_dynamicBone.m_Root = null;
				_dynamicBone.UpdateRoot();
			}
			else{
				_dynamicBone.m_Root = rigRoot;
				_dynamicBone.UpdateRoot();
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
			var collides = other.gameObject.GetComponents<IBeamCollide>();
			collides.ForEach(c => c?.OnCollide());
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
			var limitOffset = rigRoot.forward * lengthLimit;
			var localLimitOffset = rigRoot.InverseTransformVector(limitOffset);
			return (totalOffset + localAddOffset).z > localLimitOffset.z;
		}

		private void SetRigLength(int rigCount, Vector3 rigOffset){
			PostLengthUpdatedEvent(0);
			for(var i = 0; i < rigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				rigTransform.localPosition = rigOffset;
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