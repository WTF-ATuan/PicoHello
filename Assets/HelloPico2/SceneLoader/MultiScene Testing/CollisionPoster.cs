using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.SceneLoader.MultiScene_Testing{
	public class CollisionPoster : MonoBehaviour{
		[Serializable]
		public enum CollisionType{
			TriggerEnter,
			TriggerExit,
			CollisionEnter,
			CollisionExit
		}

		[EnumToggleButtons] [HideLabel] public CollisionType type;
		[SerializeField] private UnityEvent unityEvent;

		private void OnTriggerEnter(Collider other){
			if(type.Equals(CollisionType.TriggerEnter)){
				unityEvent?.Invoke();
			}
		}

		private void OnTriggerExit(Collider other){
			if(type.Equals(CollisionType.TriggerExit)){
				unityEvent?.Invoke();
			}
		}
	}
}