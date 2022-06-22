using System;
using System.Collections;
using System.Linq;
using Game.Project;
using HelloPico2.InputDevice.Scripts;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.TutorialSystem{
	public class TutorialEventPosterExample : MonoBehaviour{
		[ValueDropdown("GetAllComponent")] [SerializeField]
		private Component detectComponent;

		[SerializeField] [Range(1f, 5f)] private float updateTime = 3f;
		private ColdDownTimer _timer;

		private void Start(){
			var tryGetComponent = TryGetComponent(typeof(XRBaseInteractable), out _);
			if(!tryGetComponent) throw new Exception($"no Interactable component in {gameObject}");
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
			_timer = new ColdDownTimer(updateTime);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			var isSameObject = obj.IsSameObject(gameObject);
			if(!isSameObject) return;
			if(!_timer.CanInvoke()) return;
			var trackedData = new BehaviorTrackedData{
				IsGrip = obj.IsGrip,
				IsTrigger = obj.IsTrigger,
				TouchPadAxis = obj.TouchPadAxis
			};
			var behaviorDetected = new TutorialBehaviorDetected(detectComponent.GetType(), trackedData);
			EventBus.Post(behaviorDetected);
			_timer.Reset();
		}

		private IEnumerable GetAllComponent(){
			var components = GetComponents(typeof(Component)).ToList();
			return components.Select(x => new ValueDropdownItem(x.GetType().Name, x));
		}
	}
}