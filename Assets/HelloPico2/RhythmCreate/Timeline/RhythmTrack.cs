using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(TextAsset))]
[TrackClipType(typeof(RhythmClip))]
public class RhythmTrack : TrackAsset{
	private TextAsset _textAsset;

	private void OnEnable(){ }

	public override void GatherProperties(PlayableDirector director, IPropertyCollector driver){
		var binding = director.GetGenericBinding(this);
		_textAsset = binding as TextAsset;
		base.GatherProperties(director, driver);
	}

	protected override void OnCreateClip(TimelineClip clip){
		clip.displayName = "Note";
		base.OnCreateClip(clip);
	}
}