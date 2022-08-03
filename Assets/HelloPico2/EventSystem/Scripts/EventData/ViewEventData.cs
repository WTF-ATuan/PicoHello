using System;
using Sirenix.OdinInspector;

namespace HelloPico2{
	[Serializable]
	public class ViewEventData{
		[Required] public string identity;
		public string description;
		public virtual bool Equals(string foundID){
			return foundID.Equals(identity);
		}
	}
}