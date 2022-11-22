using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace HelloPico2.LevelTool{
	public class DebugSceneLoad : MonoBehaviour{
		[SerializeReference] [OdinSerialize] [InlineButton("GetScene")]
		public Dictionary<int, string> sceneFinder;

		private SceneController _sceneController;

		private void Start(){
			_sceneController = FindObjectOfType<SceneController>();
		}
		#if UNITY_EDITOR
		private void GetScene(){ }
		#endif
	}
}