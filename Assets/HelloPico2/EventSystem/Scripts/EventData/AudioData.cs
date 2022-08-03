using System;
using UnityEngine;

namespace HelloPico2{
	[Serializable]
	public class AudioData : ViewEventData{
		public AudioClip clip;
		public override Color GetEditorColor(){
			return new Color(0.70196f, 0.34902f, 0.1f);
		}
	}
}