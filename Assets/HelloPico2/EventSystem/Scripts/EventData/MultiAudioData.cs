﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HelloPico2{
	[Serializable]
	public class MultiAudioData : ViewEventData{
		public List<AudioClip> audioClips;
		[EnumToggleButtons] public PickRule pickRule;

		private int _index;

		public override Color GetEditorColor(){
			return new Color(0.60000f, 0.00000f, 0.00000f);
		}

		public AudioClip GetAudio(){
			switch(pickRule){
				case PickRule.Index:
					var index = GetNextIndex();
					var audioClip = audioClips[index];
					return audioClip;
				case PickRule.Random:
					var randomIndex = GetRandomIndex();
					var randomClip = audioClips[randomIndex];
					return randomClip;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private int GetNextIndex(){
			var audioClipsCount = audioClips.Count;
			_index += 1;
			if(_index >= audioClipsCount){
				_index = 0;
			}

			return _index;
		}

		private int GetRandomIndex(){
			var audioClipsCount = audioClips.Count;
			var randomValue = Random.Range(0, audioClipsCount - 1);
			return randomValue;
		}
	}

	public enum PickRule{
		Index,
		Random
	}
}