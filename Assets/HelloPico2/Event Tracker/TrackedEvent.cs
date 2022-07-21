using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.Event_Tracker{
	[Serializable]
	public class TrackedEvent{
		public string eventName;
		public List<EventParams> paramsList;

		public string GetEventJsonString(){
			var eventString = $@"""eventName"":""{eventName}"",";
			foreach(var eventParams in paramsList){
				var paramsName = eventParams.paramsName;
				var paramsValue = eventParams.paramsValue;
				eventString += $@"""{paramsName}"":""{paramsValue}"",";
			}

			var removedString = eventString.Remove(eventString.LastIndexOf(','));
			var resultString = "{" + $"{removedString}" + "}";
			return resultString;
		}

		public void SetParamsValue(string paramsName, string paramsValue){
			var eventParams = paramsList.Find(x => x.paramsName.Equals(paramsName));
			eventParams.SetValue(paramsValue);
		}

		#if UNITY_EDITOR
		[Button]
		public void DebugEventJsonString(){
			var eventString = $@"""eventName"":""{eventName}"",";
			foreach(var eventParams in paramsList){
				var paramsName = eventParams.paramsName;
				var paramsValue = eventParams.paramsValue;
				eventString += $@"""{paramsName}"":""{paramsValue}"",";
			}

			var removedString = eventString.Remove(eventString.LastIndexOf(','));
			var resultString = "{" + $"{removedString}" + "}";
			Debug.Log($"{resultString}");
		}
		#endif
	}
}