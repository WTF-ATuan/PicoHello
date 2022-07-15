using HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace HelloPico2.LevelTool{
	[RequireComponent(typeof(Button))]
	public class TimelineChangerButton : MonoBehaviour{
		[Required] public CrossSceneEventPoster eventPoster;
		private Button _button;
		private string _changedName;

		private void Start(){
			_button = GetComponent<Button>();
			_changedName = gameObject.name;
			_button.onClick.AddListener(OnButtonClick);
		}

		[Button]
		private void OnButtonClick(){
			var timelineChanged = new TimelineChanged{
				eventID = _changedName
			};
			eventPoster.Post(timelineChanged);
		}
	}
}