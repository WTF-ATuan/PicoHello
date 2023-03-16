using System.Collections.Generic;
using System.Linq;
using HelloPico2.InputDevice.Scripts;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.LevelTool{
	public class InputEventHandler : MonoBehaviour{
		public List<InputUsages> enableUsages = new List<InputUsages>();

		[BoxGroup] public UnityEvent onUsageTrigger;

		private void Start(){
			EventBus.Subscribe<DeviceInputDetected>(HandleInput);
		}

		private bool _isInvoke;
        private List<bool> triggerList = new List<bool>();
        private List<bool> containList = new List<bool>();

        private void HandleInput(DeviceInputDetected obj){
			triggerList = new List<bool>{
				obj.IsTrigger,
				obj.IsGrip,
				obj.IsPrimary,
				obj.IsSecondary,
				obj.IsMenu
			};
			containList = new List<bool>{
				enableUsages.Contains(InputUsages.TriggerButton),
				enableUsages.Contains(InputUsages.GripButton),
				enableUsages.Contains(InputUsages.PrimaryButton),
				enableUsages.Contains(InputUsages.SecondaryButton),
				enableUsages.Contains(InputUsages.MenuButton),
			};
			if(triggerList.Any(x => x == false))
				_isInvoke = false;

			if(triggerList.SequenceEqual(containList) && !_isInvoke){
				onUsageTrigger?.Invoke();
				_isInvoke = true;
			}
		}
	}

	public enum InputUsages{
		TriggerButton,
		GripButton,
		PrimaryButton,
		SecondaryButton,
		MenuButton,
		JoyStickUp,
		JoyStickDown,
	}
}