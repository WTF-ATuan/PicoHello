using System;
using HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene;
using UnityEngine;
using UnityEngine.Playables;

public class ScenePlayableBehavior : PlayableBehaviour{
	public SceneObject SceneObject;
	public LoadOptions LoadOptions;

	private bool _invokeFlag;

	public override void ProcessFrame(Playable playable, FrameData info, object playerData){
		if(_invokeFlag || !Application.isPlaying) return;
		if(SceneObject == null) throw new Exception("Scene is None Please Assign");
		var sceneController = playerData as SceneController;
		InvokeSceneAction(sceneController);
	}

	private void InvokeSceneAction(SceneController controller){
		var sceneName = SceneObject.ToString();
		switch(LoadOptions){
			case LoadOptions.Load:
				controller.LoadScene(sceneName);
				break;
			case LoadOptions.BackgroundLoad:
				controller.BackGroundLoadScene(sceneName);
				break;
			case LoadOptions.ActiveBackground:
				controller.ActiveBackgroundScene(sceneName);
				break;
			case LoadOptions.UnLoad:
				controller.UnloadScene(sceneName);
				break;
			case LoadOptions.SetMainScene:
				controller.ActiveToMainScene(sceneName);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		_invokeFlag = true;
	}

	public override void OnGraphStart(Playable playable){
		_invokeFlag = false;
		base.OnGraphStart(playable);
	}

	public override void OnGraphStop(Playable playable){
		_invokeFlag = false;
		base.OnGraphStop(playable);
	}
}