using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace HelloPico2.Interact.Sword.Scripts{
	public class Sword : MonoBehaviour{
		[SerializeField] private Transform blade;

		[ReadOnly] [SerializeField] private List<Transform> bladeComponentList;

		private DampedRig _rig;

		private void Start(){
			bladeComponentList = blade.GetComponentsInChildren<Transform>().ToList();
			_rig = GetComponent<DampedRig>();
		}

		public void AddBladeComponent(){
			//複製一份最後面的 
			var lastComponent = bladeComponentList.Last();
			var addedComponent = Instantiate(lastComponent, lastComponent.position, Quaternion.identity, lastComponent);
			//加上他的Local Position
			var lastComponentLocalPosition = lastComponent.localPosition;
			addedComponent.localPosition = lastComponentLocalPosition;
			bladeComponentList.Add(addedComponent);
			_rig.AddRig(lastComponent, addedComponent);
		}

		public void RemoveBladeComponent(){
			var lastComponent = bladeComponentList.Last();
			bladeComponentList.Remove(lastComponent);
			DestroyImmediate(lastComponent.gameObject);
			_rig.RemoveRig(lastComponent);
		}
	}
}