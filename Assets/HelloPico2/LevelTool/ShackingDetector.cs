using System;
using HelloPico2.InputDevice.Scripts;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.LevelTool{
	public class ShackingDetector : MonoBehaviour{
		[SerializeField] private float shackingSpeed = 2;
		[SerializeField] private float shackingTime = 2;
		[SerializeField] private UnityEvent onShackingDetected;
		

		[BoxGroup("Debug")] [ReadOnly]
		public float detectedTime = 0;

		private void OnEnable(){
			EventBus.Subscribe<DeviceInputDetected>(OnInputDetected);
		}

		private void OnDisable(){
			detectedTime = 0;
		}

		private void OnInputDetected(DeviceInputDetected obj){
			if(!gameObject) return;
			var selectorSpeed = obj.Selector.Speed;
			if(selectorSpeed > shackingSpeed){
				detectedTime += Time.fixedDeltaTime;
			}
			Condition();
		}

		private void Condition(){
			if(detectedTime < shackingTime) return;
			onShackingDetected?.Invoke();
			detectedTime = 0;
		}
	}
}