using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HelloPico2{
	[Serializable]
	public class MultiVFXData : ViewEventData{
		public List<ParticleSystem> vfxList;
		[EnumToggleButtons] public PickRule pickRule;

		private int _index;

		public override Color GetEditorColor(){
			return new Color(0.40000f, 0.00000f, 0.80000f);
		}

		public ParticleSystem GetParticle(){
			switch(pickRule){
				case PickRule.Index:
					var index = GetNextIndex();
					var audioClip = vfxList[index];
					return audioClip;
				case PickRule.Random:
					var randomIndex = GetRandomIndex();
					var randomClip = vfxList[randomIndex];
					return randomClip;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private int GetNextIndex(){
			var audioClipsCount = vfxList.Count;
			_index += 1;
			if(_index >= audioClipsCount){
				_index = 0;
			}

			return _index;
		}

		private int GetRandomIndex(){
			var audioClipsCount = vfxList.Count;
			var randomValue = Random.Range(0, audioClipsCount - 1);
			return randomValue;
		}
	}
}