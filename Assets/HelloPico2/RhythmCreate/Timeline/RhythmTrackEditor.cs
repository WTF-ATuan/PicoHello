#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RhythmTrack))]
public class RhythmTrackEditor : Editor{
	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		var rhythmTrack = target as RhythmTrack;
		if(rhythmTrack == null) return;
		if(GUILayout.Button("Create Select Note")){
			if(rhythmTrack.activateNoteList.Count < 1){
				throw new Exception("Selected note is empty");
			}
			if(rhythmTrack.CheckClipExists()){
				rhythmTrack.DeleteAllNote();
				rhythmTrack.CreateSelectedNote();
			}
			else{
				rhythmTrack.CreateSelectedNote();
			}
		}

		if(GUILayout.Button("Delete All Note")){
			rhythmTrack.DeleteAllNote();
		}
	}
}
#endif