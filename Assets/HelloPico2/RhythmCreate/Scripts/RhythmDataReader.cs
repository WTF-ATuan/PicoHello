using System;
using System.Collections.Generic;
using Melanchall.DryWetMidi.MusicTheory;
using UnityEngine;

namespace HelloPico2.RhythmCreate.Scripts{
	public class RhythmDataReader{
		public readonly Dictionary<NoteName, List<double>> StampDictionary = new Dictionary<NoteName, List<double>>();

		public RhythmDataReader(TextAsset textAsset){
			var originalText = textAsset.text;
			ReadTextData(originalText);
		}

		private void ReadTextData(string original){
			var lines = original.Split(new[]{ Environment.NewLine }, StringSplitOptions.None);
			for(var i = 0; i < lines.Length; i += 2){
				var j = i + 1;
				if(j >= lines.Length) return;
				var noteName = lines[i];
				Enum.TryParse<NoteName>(noteName, out var note);
				var timeStampList = new List<double>();
				var timeStampData = lines[j].Split(',');
				foreach(var timeStamp in timeStampData)
					if(double.TryParse(timeStamp, out var stamp))
						timeStampList.Add(stamp);

				StampDictionary.Add(note, timeStampList);
			}
		}
	}
}