using HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace HelloPico2.LevelTool{
	//[RequireComponent(typeof(Button))]
	public class TimelineChangerMenuItem : MonoBehaviour{
		[Required] public CrossSceneEventPoster eventPoster;
		
		public  string _changedName;

		private void Start(){
			//_button = GetComponent<Button>();
			//_button.onClick.AddListener(OnButtonClick);
		}

		public void OnButtonClick(){
			var timelineChanged = new TimelineChanged{
				eventID = _changedName
			};
			eventPoster.Post(timelineChanged);
		}
	}
}