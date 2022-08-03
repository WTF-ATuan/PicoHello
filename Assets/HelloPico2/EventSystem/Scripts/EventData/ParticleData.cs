using System;
using UnityEngine;

namespace HelloPico2{
	[Serializable]
	public class ParticleData : ViewEventData{
		public ParticleSystem particle;
		public override Color GetEditorColor(){
			return new Color(0f, 0.40000f, 0.8f);
		}
	}
}