using System;
using UnityEngine;

namespace HelloPico2.LevelTool{
	public class QuitGame : MonoBehaviour{
		private void OnEnable(){
			Application.Quit();
		}
	}
}