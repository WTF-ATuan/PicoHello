using System;

namespace HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene{
	public class CrossEventPosted{
		public Type EventType{ get; }
		public CrossEvent CrossEvent{ get; }

		public CrossEventPosted(Type eventType, CrossEvent crossEvent){
			EventType = eventType;
			CrossEvent = crossEvent;
		}
	}
}