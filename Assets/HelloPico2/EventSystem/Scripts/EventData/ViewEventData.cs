using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2{
	[Serializable]
	public class ViewEventData{
		[Required] public string identity;

		public virtual bool Equals(string foundID){
			return foundID.Equals(identity);
		}

		public virtual Color GetEditorColor(){
			return new Color(0.25f, 0.25f, 0.25f);
		}
	}
}