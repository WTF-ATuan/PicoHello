using System.Collections.Generic;
using HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene;
using UnityEngine;

namespace HelloPico2.LevelTool{
	public class TimelineChanger : MonoBehaviour , ICrossEventHandle{
		[SerializeField] private List<GameObject> timelineObjectList;


		public void Handle(CrossEvent crossEvent){
			throw new System.NotImplementedException();
		}
	}
}