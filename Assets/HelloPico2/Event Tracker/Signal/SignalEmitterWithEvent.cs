using System;
using UnityEngine.Timeline;

namespace Actor.Scripts.EventMessage{
	[Serializable]
	public class SignalEmitterWithEvent : SignalEmitter{
		public string eventName = "Event Name!!!!";
	}
}