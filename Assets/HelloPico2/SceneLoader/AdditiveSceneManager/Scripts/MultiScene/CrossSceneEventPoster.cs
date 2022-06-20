using System.Collections;
using System.Linq;
using Project;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene{
	public class CrossSceneEventPoster : MonoBehaviour{
		[TypeFilter("GetCrossEventType")] [OdinSerialize] [SerializeReference]
		public CrossEvent postedEvent;

		[Button]
		public void Post(){
			EventBus.Post(new CrossEventPosted(postedEvent.GetType(), postedEvent));
		}

		public void Post<T>(T crossEvent) where T : CrossEvent{
			EventBus.Post(new CrossEventPosted(typeof(T), crossEvent));
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