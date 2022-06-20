using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene{
	public class CrossSceneEventHandler : MonoBehaviour{
		[TypeFilter("GetCrossEventType")] [OdinSerialize]
		public CrossEvent catchEventType;

		public CrossUnityEvent crossUnityEvent;

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