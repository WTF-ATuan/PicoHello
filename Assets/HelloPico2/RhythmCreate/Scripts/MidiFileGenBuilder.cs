using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.RhythmCreate.Scripts{
	public class MidiFileGenBuilder : MonoBehaviour{
		[FilePath] public string readPath;

		[EnumPaging] [InlineButton("Read")] public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;

		[HideLabel] [FolderPath] [HorizontalGroup("Save")]
		public string savePath;

		[HideLabel] [InlineButton("Save")] [HorizontalGroup("Save")]
		public string fileName;

		[ReadOnly] public List<double> timeStamps = new List<double>();


		private void Read(){
			if(readPath.Length < 1) return;
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
		}
	}
}