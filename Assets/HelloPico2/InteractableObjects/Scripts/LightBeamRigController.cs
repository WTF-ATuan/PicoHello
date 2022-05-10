using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace HelloPico2.InteractableObjects{
	public class LightBeamRigController : MonoBehaviour{
		[SerializeField] [Required] private Transform rigRoot;
		[SerializeField] [MinValue(0.1f)] private float maxRigDistance = 0.25f;
		private List<Transform> _rigs;
		[SerializeField] private int currentControlIndex = 5;

		private void Start(){
			_rigs = rigRoot.GetComponentsInChildren<Transform>().ToList();
			_rigs.RemoveAt(0);
		}

		private LightBeamLenghtUpdated GetDefaultUpdatedEvent(){
			var lenghtUpdated = new LightBeamLenghtUpdated();
			return lenghtUpdated;
		}

		[Button]
		public void ModifyControlRigLenght(float rigLenght){
			for(var i = 0; i < currentControlIndex; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var addPosition = rigTransform.InverseTransformVector(rigTransform.forward * rigLenght);
				var finalPosition = rigTransform.localPosition + addPosition;
				rigTransform.localPosition = finalPosition;
			}
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
			for(var i = 0; i < currentControlIndex; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var targetPosition = rigTransform.InverseTransformVector(rigTransform.forward * multiplyValue);
				var lerpPosition = Vector3.Lerp(Vector3.zero, targetPosition, value);
				rigTransform.localPosition = lerpPosition;
			}
		}
	}

	public class LightBeamLenghtUpdated{
		public float TotalLenght;
		public float SingleLenght;
		public bool IsUpdating;
	}
}