using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(Level_FadeController))]
[TrackClipType(typeof(LevelFadePlayableAsset))]
public class LevelFadeTrack : TrackAsset{
	protected override void OnCreateClip(TimelineClip clip){
		clip.displayName = "Level";
		base.OnCreateClip(clip);
	}
}
