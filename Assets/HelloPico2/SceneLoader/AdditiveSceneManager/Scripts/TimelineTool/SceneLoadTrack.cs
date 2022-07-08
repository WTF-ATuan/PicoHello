using UnityEngine.Timeline;

[TrackBindingType(typeof(SceneController))]
[TrackClipType(typeof(ScenePlayableAsset))]
[TrackColor(1, 0, 0)]
public class SceneLoadTrack : TrackAsset{
	protected override void OnCreateClip(TimelineClip clip){
		clip.displayName = "Load Scene";
		base.OnCreateClip(clip);
	}
}