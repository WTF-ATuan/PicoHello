using System;
using System.Collections.Generic;
using System.Linq;
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
			var lineLength = lines.Length / 2;
			for(var i = 0; i < lineLength; i += 2){
				var noteName = lines[i];
				Enum.TryParse<NoteName>(noteName, out var note);
				var timeStampList = new List<double>();
				for(var j = 1; j < lineLength; j += 2){
					var timeStampData = lines[j].Split(',');
					timeStampList.AddRange(timeStampData.Select(double.Parse));
				}

				StampDictionary.Add(note, timeStampList);
			}
		}
	}
}