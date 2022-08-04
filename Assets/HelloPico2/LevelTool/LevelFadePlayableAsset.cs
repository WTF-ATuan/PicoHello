using UnityEngine;
using UnityEngine.Playables;

public class LevelFadePlayableAsset : PlayableAsset{
	public Level_FadeController.Level level;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner){
		var playable = ScriptPlayable<LevelFadePlayable>.Create(graph);
		var behaviour = playable.GetBehaviour();
		behaviour.SelectLevel = level;
		return playable;
	}
}

public class LevelFadePlayable : PlayableBehaviour{
	public Level_FadeController.Level SelectLevel;

	public override void ProcessFrame(Playable playable, FrameData info, object playerData){
		var fadeController = (Level_FadeController)playerData;
		fadeController.level = SelectLevel;
	}
}