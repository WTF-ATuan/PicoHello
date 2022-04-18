using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace HelloPico2.Interact.Sword.Scripts{
	public class DampedRig : MonoBehaviour{
		[SerializeField] private Transform rig;
		private List<DampedTransform> _dampedTransformList;

		private void Start(){
			_dampedTransformList = rig.GetComponentsInChildren<DampedTransform>().ToList();
		}

		public void AddRig(Transform parent, Transform current){
			var addedObject = Instantiate(new GameObject(), current.position, current.rotation, rig);
			var dampedComponent = addedObject.AddComponent<DampedTransform>();
			dampedComponent.data.sourceObject = parent;
			dampedComponent.data.constrainedObject = current;
			dampedComponent.data.dampPosition = 0.5f;
			dampedComponent.data.dampRotation = 0.5f;
			dampedComponent.data.maintainAim = true;
			_dampedTransformList.Add(dampedComponent);
		}

		public void RemoveRig(Transform current){
			var dampedComponent = _dampedTransformList.Find(x => x.data.sourceObject == current);
			_dampedTransformList.Remove(dampedComponent);
			DestroyImmediate(dampedComponent);
		}
	}
}