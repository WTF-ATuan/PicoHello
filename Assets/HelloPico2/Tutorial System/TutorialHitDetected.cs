using System;
using System.Collections.Generic;
using System.Linq;
using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.TutorialSystem{
	public class TutorialHitDetected : MonoBehaviour, IInteractCollide{
		[ReadOnly] public int currentIndex;

		[InlineButton("Create", "Create New ")] [InlineButton("RemoveLast", "Remove at last")] [HideLabel]
		public List<HitDetectedDataWrapper> dataWrappers;

		public UnityEvent failEventList;

		private void Create(){
			var dataWrapper = new HitDetectedDataWrapper();
			dataWrappers.Add(dataWrapper);
		}

		private void RemoveLast(){
			var dataWrapper = dataWrappers.Last();
			dataWrappers.Remove(dataWrapper);
		}

		public void OnCollide(InteractType type, Collider selfCollider){
			var wrapper = dataWrappers[currentIndex];
			var interactType = wrapper.interactType;
			var successEvent = wrapper.unityEvent;
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

	[Serializable]
	public class HitDetectedDataWrapper{
		[BoxGroup("Type")] [HideLabel] [GUIColor("GetGUIColor")] [EnumToggleButtons]
		public InteractType interactType;

		[BoxGroup("Event")] [GUIColor("GetGUIColor")]
		public UnityEvent unityEvent;

		private bool _isPass = false;

		private Color GetGUIColor(){
			return _isPass ? Color.green : Color.white;
		}
	}
}