using System;
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

		private void Start(){
			_rigs = rigRoot.GetComponentsInChildren<Transform>().ToList();
			_rigs.RemoveAt(0);
		}

		[Button]
		public void ModifyPositionLenght(float multiplyValue){
			for(var i = 0; i < _currentControlIndex; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var addPosition = rigTransform.InverseTransformVector(rigTransform.forward * multiplyValue);
				rigTransform.DOLocalMove(addPosition, multiplyValue);
			}
		}

		[Button]
		public void DynamicScale(){
			for(var i = 0; i < _currentControlIndex; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				rigTransform.DOScale(Vector3.one * 2, i)
						.OnComplete(() => { rigTransform.DOScale(Vector3.one, _currentControlIndex - i); });
			}
		}

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