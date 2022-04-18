using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project{
	public class SceneLoader : MonoBehaviour{
		private readonly List<AsyncOperation> _sceneToLoad = new List<AsyncOperation>();

		public void AddScene(string sceneName){ }
	}
}