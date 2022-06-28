using System.Collections.Generic;
using Melanchall.DryWetMidi.MusicTheory;
using UnityEngine;

namespace HelloPico2.RhythmCreate.Scripts{
	public class RhythmDataReader{
		public readonly Dictionary<NoteName, List<double>> StampDictionary;

		public RhythmDataReader(TextAsset textAsset){
			StampDictionary = new Dictionary<NoteName, List<double>>();
			var originalText = textAsset.text;
			ReadTextData(originalText);
		}

		private void ReadTextData(string original){
			var dataSplit = original.Split('\n');
			foreach(var data in dataSplit){
				var timeStamps = double.Parse(data); //Bug
				var containsKey = StampDictionary.ContainsKey(NoteName.C);
				if(containsKey){
					StampDictionary[NoteName.C].Add(timeStamps);
				}
				else{
					StampDictionary.Add(NoteName.C, new List<double>{ timeStamps });
				}
			}
		}
	}
}