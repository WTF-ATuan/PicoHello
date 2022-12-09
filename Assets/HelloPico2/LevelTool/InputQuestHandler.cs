using System;
using System.Collections.Generic;
using System.Linq;
using HelloPico2.InputDevice.Scripts;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.LevelTool{
	public class InputQuestHandler : MonoBehaviour{
		public HandType detectHand;
		public List<QuestEvent> questEvents = new List<QuestEvent>();

		private Dictionary<InputUsages, bool> _triggerData
				= new Dictionary<InputUsages, bool>{
					{ InputUsages.TriggerButton, false },
					{ InputUsages.GripButton, false },
					{ InputUsages.JoyStickUp, false },
					{ InputUsages.JoyStickDown, false }
				};

		private void Start(){
			EventBus.Subscribe<DeviceInputDetected>(OnInputDetected);
		}

		private void OnInputDetected(DeviceInputDetected obj){
			if(questEvents.Count < 1){
				return;
			}

			if(obj.Selector.HandType != detectHand) return;
			SetTriggerData(obj);
			RunQuest();
		}

		private void RunQuest(){
			var notPassQuest = questEvents.FindAll(x => x.isPass == false).First();
			if(_triggerData[notPassQuest.inputUsages] && !notPassQuest.IsClickDown){
				notPassQuest.clickDown?.Invoke();
				notPassQuest.IsClickDown = true;
				return;
			}

			if(!_triggerData[notPassQuest.inputUsages] && notPassQuest.IsClickDown){
				notPassQuest.clickUp?.Invoke();
				notPassQuest.isPass = true;
			}
		}

		private void SetTriggerData(DeviceInputDetected obj){
			_triggerData[InputUsages.TriggerButton] = obj.IsTrigger;
			_triggerData[InputUsages.GripButton] = obj.IsGrip;
			_triggerData[InputUsages.JoyStickUp] = obj.TouchPadAxis.y > 0;
			_triggerData[InputUsages.JoyStickDown] = obj.TouchPadAxis.y < 0;
		}

		[Button]
		private void RestartQuest(){
			questEvents.ForEach(x => x.Restart());
		}
	}

	[Serializable]
	public class QuestEvent{
		[HorizontalGroup] [HideLabel] public InputUsages inputUsages;
		[HorizontalGroup] [ReadOnly] public bool isPass;
		[HorizontalGroup("Event")] public UnityEvent clickDown;
		[HorizontalGroup("Event")] public UnityEvent clickUp;
		public bool IsClickDown{ get; set; }

		public void Restart(){
			IsClickDown = false;
			isPass = false;
		}
	}
}