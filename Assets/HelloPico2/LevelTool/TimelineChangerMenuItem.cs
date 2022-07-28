using HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace HelloPico2.LevelTool{
	//[RequireComponent(typeof(Button))]
	public class TimelineChangerMenuItem : MonoBehaviour{
		[Required] public CrossSceneEventPoster eventPoster;
		
		public  string _changedName;
		public int m_DelayTime;
		private void Start(){
			//_button = GetComponent<Button>();
			//_button.onClick.AddListener(OnButtonClick);
		}

		public void OnButtonClick(){
			var timelineChanged = new TimelineChanged{
				eventID = _changedName
			};
			StartCoroutine(Delayer());
			eventPoster.Post(timelineChanged);
		}
		private System.Collections.IEnumerator Delayer()
		{
			yield return new WaitForSeconds(m_DelayTime);
			
		}
	}
}