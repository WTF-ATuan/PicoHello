using System;
using System.Collections.Generic;
using HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene;
using UnityEngine;

namespace HelloPico2.LevelTool{
	public class TimelineChanger : MonoBehaviour{
		[SerializeField] private List<GameObject> timelineObjectList;


		public void Handle(CrossEvent crossEvent){
			var timelineChanged = (TimelineChanged)crossEvent;
			var timelineID = timelineChanged.eventID;
			if(string.IsNullOrEmpty(timelineID)){
				throw new Exception("Can,t be Null");
			}

			ChangeTimeline(timelineID);
		}

		private void ChangeTimeline(string timelineID){
			var foundTimeline = timelineObjectList.Find(x => x.name.Equals(timelineID));
			if(!foundTimeline){
				throw new Exception($"not found {timelineID} , please check");
			}

			timelineObjectList.ForEach(x => x.SetActive(false));
			foundTimeline.SetActive(true);
		}
	}
}