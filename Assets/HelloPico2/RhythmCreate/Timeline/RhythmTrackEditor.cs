#if UNITY_EDITOR
using System;
using Melanchall.DryWetMidi.MusicTheory;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RhythmTrack))]
public class RhythmTrackEditor : Editor{
	private NoteName _selectNoteName;
	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		ReadNoteButton();
		CreateNote();
	}

	private void CreateNote(){
		GUILayout.Label("New Note");
		var rhythmTrack = target as RhythmTrack;
		if(rhythmTrack == null) return;
		GUILayout.BeginHorizontal();
		_selectNoteName = (NoteName)EditorGUILayout.EnumPopup(_selectNoteName);
		if(GUILayout.Button("Create New Note")){
			rhythmTrack.CreateNewNote(_selectNoteName);
		}
		GUILayout.EndHorizontal();
	}

	private void ReadNoteButton(){
		var rhythmTrack = target as RhythmTrack;
		if(rhythmTrack == null) return;
		GUILayout.BeginHorizontal();
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
		GUILayout.EndHorizontal();
	}
}
#endif