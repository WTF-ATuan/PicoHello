using System;
using System.Collections.Generic;
using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.TutorialSystem{
	public class TutorialHitDetected : MonoBehaviour, IInteractCollide{
		[ReadOnly] public int currentIndex;

		[EnumToggleButtons] [InlineButton("Create", "Create New Detecting")]
		public List<InteractType> interactList;

		[InlineButton("Create", "Create New Detecting")]
		public List<UnityEvent> successEventList;

		public UnityEvent failEventList;

		[PropertyOrder(-1)]
		private void Create(){
			var baseType = InteractType.Beam;
			var unityEvent = new UnityEvent();
			interactList.Add(baseType);
			successEventList.Add(unityEvent);
		}

		public void OnCollide(InteractType type, Collider selfCollider){
			var interactType = interactList[currentIndex];
			var successEvent = successEventList[currentIndex];
			if(interactType.Equals(type)){
				successEvent?.Invoke();
				currentIndex++;
			}
			else{
				failEventList?.Invoke();
			}
		}

		public Action<InteractType, Collider> ColliderEvent => null;
	}
}