using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HelloPico2.LevelTool{
	public class DebugSceneLoad : MonoBehaviour{
		[SerializeReference] [OdinSerialize] [InlineButton("GetScene")]
		public Dictionary<int, string> sceneFinder = new Dictionary<int, string>();

		private SceneController _sceneController;

		private void Start(){
			_sceneController = FindObjectOfType<SceneController>();
		}

		private void GetScene(){
			sceneFinder.Clear();
			var sceneCount = SceneManager.sceneCountInBuildSettings;
			for(var index = 0; index < sceneCount; index++){
				sceneFinder.Add(index,
					System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(index)));
			}
		}
	}
}