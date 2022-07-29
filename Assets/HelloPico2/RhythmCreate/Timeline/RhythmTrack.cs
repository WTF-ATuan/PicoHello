using System.Collections.Generic;
using System.Linq;
using HelloPico2.RhythmCreate.Scripts;
using Melanchall.DryWetMidi.MusicTheory;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(TextAsset))]
[TrackClipType(typeof(RhythmClip))]
public class RhythmTrack : TrackAsset{
	private TextAsset _textAsset;
	private RhythmDataReader _dataReader;

	public List<NoteName> activateNoteList;

	public bool CheckClipExists(){
		var timelineClips = GetClips().ToList();
		return timelineClips.Count > 1;
	}

	public void CreateSelectedNote(){
		if(!_textAsset) return;
		_dataReader = new RhythmDataReader(_textAsset);
		foreach(var note in _dataReader.StampDictionary){
			var noteKey = note.Key;
			if(!activateNoteList.Contains(noteKey)) continue;
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

	public void DeleteAllNote(){
		foreach(var clip in GetClips()) DeleteClip(clip);
	}

	public override void GatherProperties(PlayableDirector director, IPropertyCollector driver){
		var binding = director.GetGenericBinding(this);
		_textAsset = binding as TextAsset;
		base.GatherProperties(director, driver);
	}
}