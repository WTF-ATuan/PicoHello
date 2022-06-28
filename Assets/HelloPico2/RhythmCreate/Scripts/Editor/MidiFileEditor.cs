using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace HelloPico2.RhythmCreate.Scripts.Editor{
	public class MidiFileEditor : OdinEditorWindow{
		[MenuItem("Tools/Pico/MidiFileEditor")]
		private static void OpenWindow(){
			GetWindow<MidiFileEditor>().Show();
		}

		[Title("Read Data")] [Required] [BoxGroup] [Sirenix.OdinInspector.FilePath] [InlineButton("Read")]
		public string readPath;

		[HideLabel] [FolderPath] [HorizontalGroup("Save")] [PropertyOrder(2)] [Title("Save Data")]
		public string savePath;

		[HideLabel] [InlineButton("Save")] [HorizontalGroup("Save")] [PropertyOrder(2)] [Title("Save Data")]
		public string fileName;

		[OdinSerialize]
		[BoxGroup]
		[ReadOnly]
		[DictionaryDrawerSettings(KeyLabel = "Note Name", ValueLabel = "Time Stamp Values")]
		public Dictionary<NoteName, List<double>> noteStampDictionary = new Dictionary<NoteName, List<double>>();

		[AssetList(Path = "/Resources/RhythmCreateData/")] [PropertyOrder(3)]
		public List<TextAsset> dataFileList;


		private void Read(){
			if(readPath.Length < 1) return;
			noteStampDictionary.Clear();
			var midiFile = MidiFile.Read(readPath);
			var midiFileNotes = midiFile.GetNotes();
			var notes = new Melanchall.DryWetMidi.Interaction.Note[midiFileNotes.Count];
			var tempoMap = midiFile.GetTempoMap();
			midiFileNotes.CopyTo(notes, 0);
			foreach(var note in notes){
				var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
				if(noteStampDictionary.ContainsKey(note.NoteName)){
					noteStampDictionary[note.NoteName].Add((double)metricTimeSpan.Minutes * 60f +
														   metricTimeSpan.Seconds +
														   (double)metricTimeSpan.Milliseconds / 1000f);
				}
				else{
					var stampList = new List<double>{
						(double)metricTimeSpan.Minutes * 60f +
						metricTimeSpan.Seconds +
						(double)metricTimeSpan.Milliseconds / 1000f
					};
					noteStampDictionary.Add(note.NoteName, stampList);
				}
			}

			Debug.Log("Read <color=#00FF00>Complete</color>;");
		}

		private void Save(){
			var filePath = savePath + $"/{fileName}.txt";
			var fileStream = File.Create(filePath);
			var writer = new StreamWriter(fileStream);
			foreach(var noteStamps in noteStampDictionary){
				var noteName = noteStamps.Key;
				writer.Write(noteName + Environment.NewLine);
				foreach(var timeStamp in noteStamps.Value)
					writer.Write(timeStamp.ToString(CultureInfo.InvariantCulture) + ",");

				writer.Write(Environment.NewLine);
			}

			writer.Close();
			fileStream.Close();
			Debug.Log("Write <color=#00FF00>Complete</color>;");
			AssetDatabase.Refresh();
		}
	}
}