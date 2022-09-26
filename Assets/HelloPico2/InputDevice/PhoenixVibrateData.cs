using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.InputDevice{
	[CreateAssetMenu(fileName = "Phoenix Vibrate Data",
		menuName = "HelloPico2/ScriptableObject/ Phoenix Vibrate Data",
		order = 0)]
	public class PhoenixVibrateData : ScriptableObject{
		public List<VibrateData> vibrateDataList;

		public AudioClip FindClip(string vibrateName){
			var vibrateData = vibrateDataList.Find(x => x.vibrateName.Equals(vibrateName));
			if(vibrateData == null) throw new Exception($"{vibrateName} is not found in {this}");
			return vibrateData.vibrateClip;
		}
	}
	[System.Serializable]
	public class VibrateData{
		public string vibrateName = "None";
		public AudioClip vibrateClip;
	}
}