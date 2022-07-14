using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project;
using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;

namespace HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene{
	public class CrossSceneEventHandler : MonoBehaviour{
		public List<Setting> crossEventList;

		private void Start(){
			EventBus.Subscribe<CrossEventPosted>(OnCrossEventPosted);
		}

		private void OnCrossEventPosted(CrossEventPosted obj){
			var postedType = obj.EventType;
			var crossEvent = obj.CrossEvent;
			var eventID = crossEvent.eventID;
			var catchTypeEvents = crossEventList.FindAll(x => x.catchEventType.GetType() == postedType);
			if(string.IsNullOrEmpty(eventID)){
				catchTypeEvents.ForEach(x => x.crossUnityEvent?.Invoke(crossEvent));
			}
			else{
				var foundEvent = catchTypeEvents.Find(x => x.eventID.Equals(eventID));
				var nonBindingEvent = catchTypeEvents.Find(x => string.IsNullOrEmpty(x.eventID));
				foundEvent?.crossUnityEvent?.Invoke(crossEvent);
				nonBindingEvent?.crossUnityEvent?.Invoke(crossEvent);
			}
		}

		public void InvokeTest(CrossEvent crossEvent){
			var type = crossEvent.GetType();
			var typeName = type.Name;
			var eventID = crossEvent.eventID;
			Debug.Log($"test event - {typeName} - is invoke with {eventID}");
		}

		private IEnumerable GetCrossEventType(){
			var dropdownItems = typeof(CrossEvent).Assembly
					.GetTypes()
					.Where(x => !x.IsAbstract)
					.Where(x => !x.IsGenericTypeDefinition)
					.Where(x => typeof(CrossEvent).IsAssignableFrom(x));
			return dropdownItems;
		}

		[Serializable]
		public class Setting{
			[TypeFilter("GetCrossEventType")]
			[SerializeReference]
			[HorizontalGroup("Event")]
			[LabelWidth(100)]
			[LabelText("Type")]
			public CrossEvent catchEventType;

			[HorizontalGroup("Event")] [LabelText("ID")] [LabelWidth(30)]
			public string eventID;

			public UltEvent<CrossEvent> crossUnityEvent;

			[SerializeField] private bool testing;

			[SerializeField] [SerializeReference] [BoxGroup("Test")] [ShowIf("testing")] [HideLabel]
			private CrossEvent testEventArg;

			[Button]
			[ShowIf("testing")]
			[BoxGroup("Test")]
			public void InvokeTest(){
				crossUnityEvent?.Invoke(testEventArg);
			}

			private IEnumerable GetCrossEventType(){
				var dropdownItems = typeof(CrossEvent).Assembly
						.GetTypes()
						.Where(x => !x.IsAbstract)
						.Where(x => !x.IsGenericTypeDefinition)
						.Where(x => typeof(CrossEvent).IsAssignableFrom(x));
				return dropdownItems;
			}
		}
	}
}