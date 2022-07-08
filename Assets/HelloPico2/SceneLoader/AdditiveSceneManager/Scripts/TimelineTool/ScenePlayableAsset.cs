using System;
using HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

public class ScenePlayableAsset : PlayableAsset{
	[EnumToggleButtons] public LoadOptions loadOption;
	[Required] public SceneObject scene;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner){
		var playable = ScriptPlayable<ScenePlayableBehavior>.Create(graph);
		var behaviour = playable.GetBehaviour();
		behaviour.SceneObject = scene;
		behaviour.LoadOptions = loadOption;
		return playable;
	}
}