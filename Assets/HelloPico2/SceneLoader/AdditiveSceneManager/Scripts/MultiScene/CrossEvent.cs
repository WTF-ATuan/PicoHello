using System;
using UnityEngine.Events;

namespace HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene{
	[Serializable]
	public class CrossEvent{ }


	[Serializable]
	public class SceneLoaded : CrossEvent{
		public SceneObject sceneObject;
		public LoadOptions options;
		public float delayTime;
	}

	[Serializable]
	public class CrossUnityEvent : UnityEvent<CrossEvent>{ }
}