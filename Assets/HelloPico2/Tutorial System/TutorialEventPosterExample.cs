using System;
using System.Collections;
using System.Linq;
using HelloPico2.InputDevice.Scripts;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.TutorialSystem{
	public class TutorialEventPosterExample : MonoBehaviour{
		[ValueDropdown("GetAllComponent")] [SerializeField]
		private Component detectComponent;

		private void Start(){
			var tryGetComponent = TryGetComponent(typeof(XRBaseInteractable), out var component);
			if(!tryGetComponent) throw new Exception($"no Interactable component in {gameObject}");
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){ }

		private IEnumerable GetAllComponent(){
			var components = GetComponents(typeof(Component)).ToList();
			return components.Select(x => new ValueDropdownItem(x.GetType().Name, x));
		}
	}
}