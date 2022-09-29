using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Actor.Scripts.EventMessage{
	public class SignalReceiverWithEvent : MonoBehaviour, INotificationReceiver{
		public SignalAssetEventPair[] signalAssetEventPairs;

		[Serializable]
		public class SignalAssetEventPair
		{
			public SignalAsset signalAsset;
			public ParameterizedEvent events;

			[Serializable]
			public class ParameterizedEvent : UnityEvent<string> { }
		}

		public void OnNotify(Playable origin, INotification notification, object context)
		{
			if(notification is SignalEmitterWithEvent eventEmitter)
			{
				var matches = signalAssetEventPairs.Where(x => ReferenceEquals(x.signalAsset, eventEmitter.asset));
				foreach (var m in matches)
				{
					m.events.Invoke(eventEmitter.eventName);
				}
			}
		}
	}
}