using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace HelloPico2.RhythmCreate.Scripts.Editor{
	public class MidiFileEditor : OdinEditorWindow{
		[MenuItem("Tools/Pico/MidiFileEditor")]
		private static void OpenWindow(){
			GetWindow<MidiFileEditor>().Show();
		}

		[Title("Read Data")] [Required] [BoxGroup] [Sirenix.OdinInspector.FilePath]
		public string readPath;

		[EnumPaging] [InlineButton("Read")] [BoxGroup]
		public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;

		[HideLabel] [FolderPath] [HorizontalGroup("Save")] [PropertyOrder(2)] [Title("Save Data")]
		public string savePath;

		[HideLabel] [InlineButton("Save")] [HorizontalGroup("Save")] [PropertyOrder(2)] [Title("Save Data")]
		public string fileName;

		[ReadOnly] [BoxGroup] public List<double> timeStamps = new List<double>();

		[AssetList(Path = "/Resources/RhythmCreateData/")] [PropertyOrder(3)]
		public List<TextAsset> dataFileList;


		private void Read(){
			if(readPath.Length < 1) return;
			timeStamps.Clear();
			var midiFile = MidiFile.Read(readPath);
			var midiFileNotes = midiFile.GetNotes();
			var notes = new Melanchall.DryWetMidi.Interaction.Note[midiFileNotes.Count];
			var tempoMap = midiFile.GetTempoMap();
			midiFileNotes.CopyTo(notes, 0);
			foreach(var note in notes){
				if(note.NoteName != noteRestriction) continue;
				var metricTimeSpan =
						TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
				timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds +
							   (double)metricTimeSpan.Milliseconds / 1000f);
			}

			Debug.Log("Read <color=#00FF00>Complete</color>;");
		}

		private void Save(){
			var filePath = savePath + $"/{fileName}.txt";
			var fileStream = File.Create(filePath);
			var writer = new StreamWriter(fileStream);
			foreach(var timeStamp in timeStamps){
				writer.WriteLine(timeStamp.ToString(CultureInfo.InvariantCulture));
			}

			writer.Close();
			fileStream.Close();
			Debug.Log("Write <color=#00FF00>Complete</color>;");
			AssetDatabase.Refresh();
		}
	}
}