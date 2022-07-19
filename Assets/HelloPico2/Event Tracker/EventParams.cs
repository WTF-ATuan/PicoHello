using System;

namespace HelloPico2.Event_Tracker{
	[Serializable]
	public class EventParams{
		public string paramsName;
		public string paramsValue;

		public void SetValue(int value){
			var castValue = value.ToString();
			paramsValue = castValue;
		}
	}
}