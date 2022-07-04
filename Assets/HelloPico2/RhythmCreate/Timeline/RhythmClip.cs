using Melanchall.DryWetMidi.MusicTheory;
using UnityEngine;
using UnityEngine.Playables;

public class RhythmClip : PlayableAsset{
	public NoteName noteName;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner){
		var playable = ScriptPlayable<RhythmBehaviour>.Create(graph);
		var behaviour = playable.GetBehaviour();
		behaviour.NoteName = noteName;
		return playable;
	}
}