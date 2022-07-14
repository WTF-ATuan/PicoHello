using System.Collections;
using System.Linq;
using Project;
using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;

namespace HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene{
	public class CrossSceneEventHandler : MonoBehaviour{
		[TypeFilter("GetCrossEventType")] [SerializeReference] [HorizontalGroup("Event")]
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

		private void Start(){
			EventBus.Subscribe<CrossEventPosted>(OnCrossEventPosted);
		}

		private void OnCrossEventPosted(CrossEventPosted obj){
			var postedType = obj.EventType;
			var catchType = catchEventType.GetType();
			if(postedType != catchType) return;
			var crossEvent = obj.CrossEvent;
			crossUnityEvent?.Invoke(crossEvent);
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