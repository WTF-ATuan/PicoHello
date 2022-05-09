using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamRigController : MonoBehaviour{
		[SerializeField] [Required] private Transform rigRoot;
		private List<Transform> _rigs;
		private int _currentControlIndex = 5;

		public bool IsLenghtUpdating{ get; private set; }

		private void Start(){
			_rigs = rigRoot.GetComponentsInChildren<Transform>().ToList();
			_rigs.RemoveAt(0);
		}

		public float GetAllRigLenght(){
			float lenght = 0;
			for(var i = 0; i < _currentControlIndex; i++){
				var rig = _rigs[i];
				var localPosition = rig.transform.localPosition;
				lenght += localPosition.z;
			}

			return lenght;
		}

		public float GetSingleRigLenght(){
			var first = _rigs.First();
			return first.position.z;
		}

		[Button]
		public void ModifyPositionLenght(float multiplyValue){
			for(var i = 0; i < _currentControlIndex; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var addPosition = rigTransform.InverseTransformVector(rigTransform.forward * multiplyValue);
				var finalPosition = rigTransform.localPosition + addPosition;
				IsLenghtUpdating = true;
				rigTransform.localPosition = finalPosition;
			}

			IsLenghtUpdating = false;
		}

		public void SetPositionLenghtByPercent(float multiplyValue, float value){
			for(var i = 0; i < _currentControlIndex; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var targetPosition = rigTransform.InverseTransformVector(rigTransform.forward * multiplyValue);
				var lerpPosition = Vector3.Lerp(Vector3.zero, targetPosition, value);
				rigTransform.localPosition = lerpPosition;
			}
		}

		[Button]
		public void SetControlIndex(int value){
			var overLimit = value > _rigs.Count;
			if(overLimit){
				value = _rigs.Count;
				_currentControlIndex = value;
			}
			else{
				_currentControlIndex = value;
			}
		}
	}
}