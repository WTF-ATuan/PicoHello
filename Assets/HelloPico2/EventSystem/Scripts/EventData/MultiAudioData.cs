using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HelloPico2{
	[Serializable]
	public class MultiAudioData : ViewEventData{
		public List<AudioClip> clipList;

		private int _idNumber;

		public override bool Equals(string foundID){
			if(!foundID.Contains(identity)){
				return false;
			}

			var resultString = Regex.Match(foundID, @"\d+").Value;
			var number = string.IsNullOrEmpty(resultString) ? 0 : int.Parse(resultString);
			var clipListCount = clipList.Count;
			if(number > clipListCount) return false;
			_idNumber = number;
			return true;
		}

		public AudioClip GetItem(){
			if(clipList.Count < _idNumber){
				throw new Exception("ClipList Count is Less than Getter Number");
			}

			var audioClip = clipList[_idNumber];
			return audioClip;
		}
	}
}