using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.AdditiveSceneManager.Scripts{
	[CreateAssetMenu(fileName = "MultiSceneDataOverview",
		menuName = "HelloPico2/ScriptableObject/ MultiScene-DataOverview")]
	public class MultiSceneDataOverview : ScriptableObject{
		[SerializeField] private List<MultiSceneData> multiSceneDataList = new List<MultiSceneData>();

		public List<SceneObject> FindScene(SceneObject sceneKey){
			var sceneName = sceneKey.ToString();
			var sceneFound = new List<SceneObject>();
			var multiSceneData = multiSceneDataList.Find(x => x.sceneKey == sceneName);
			if(multiSceneData == null) return sceneFound;
			sceneFound = multiSceneData.sceneValue;
			return sceneFound;
		}
	}

	[Serializable]
	public class MultiSceneData{
		[Required] public SceneObject sceneKey;
		public List<SceneObject> sceneValue = new List<SceneObject>();
	}
}