using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2{
	[CreateAssetMenu(fileName = "ViewEventDataOverview",
		menuName = "HelloPico2/ScriptableObject/ ViewEventData Overview",
		order = 0)]
	public class ViewEventDataOverview : ScriptableObject{
		[Button]
		[PropertyOrder(0)]
		public void CreateAudioEvent(){
			var audioEvent = new AudioData();
			viewEventDataList.Add(audioEvent);
		}

		[SerializeReference] [PropertyOrder(100)]
		private List<ViewEventData> viewEventDataList = new List<ViewEventData>();

		public T FindEventData<T>(string id) where T : ViewEventData{
			var viewEventData = viewEventDataList.Find(x => x.identity.Equals(id));
			return viewEventData as T;
		}

		// (使用者) 傳送 我需要聲音事件 (音效ID) => Handler 接收事件 來 OverView 當中找聲音事件 => Handler 幫忙撥出音效 or 給另一個角色播音效。
	}
}