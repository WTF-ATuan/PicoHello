using HelloPico2.RhythmCreate.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(TextAsset))]
[TrackClipType(typeof(RhythmClip))]
public class RhythmTrack : TrackAsset{
	private TextAsset _textAsset;
	private RhythmDataReader _dataReader;
	
	private void CreateAllNote(){
		if(!_textAsset) return;
		_dataReader = new RhythmDataReader(_textAsset);
		foreach(var clip in GetClips()){
			DeleteClip(clip);
		}

		foreach(var note in _dataReader.StampDictionary){
			var noteKey = note.Key;
			var timeList = note.Value;
			foreach(var timeStamp in timeList){
				var timelineClip = CreateClip<RhythmClip>();
				timelineClip.displayName = noteKey.ToString();
				timelineClip.start = timeStamp;
				timelineClip.duration = 1f;
				var rhythmClip = timelineClip.asset as RhythmClip;
				if(rhythmClip != null)
					rhythmClip.noteName = noteKey;
			}
		}
	}
	public override void GatherProperties(PlayableDirector director, IPropertyCollector driver){
		var binding = director.GetGenericBinding(this);
		_textAsset = binding as TextAsset;
		CreateAllNote();
		base.GatherProperties(director, driver);
	}
}