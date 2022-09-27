using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.InputDevice{
	[CreateAssetMenu(fileName = "Phoenix Vibrate Data",
		menuName = "HelloPico2/ScriptableObject/ Phoenix Vibrate Data",
		order = 0)]
	public class VibrateData : ScriptableObject{
		public List<PhoenixVibrateData> phoenixVibrateDataList;

		public List<Neo3VibrateData> neo3VibrateDataList;

		public AudioClip FindClip(string vibrateName){
			var vibrateData = phoenixVibrateDataList.Find(x => x.vibrateName.Equals(vibrateName));
			if(vibrateData == null) throw new Exception($"Phoenix {vibrateName} is not found in {this}");
			return vibrateData.vibrateClip;
		}

		public void GetNeo3Vibrate(string vibrateName, out float amplitude, out float time){
			var vibrateData = neo3VibrateDataList.Find(x => x.vibrateName.Equals(vibrateName));
			if(vibrateData == null) throw new Exception($"Neo3 {vibrateName} is not found in {this}");
			amplitude = vibrateData.amplitude;
			time = vibrateData.time;
		}
	}

	[Serializable]
	public class PhoenixVibrateData{
		public string vibrateName = "None";
		public AudioClip vibrateClip;
	}

	[Serializable]
	public class Neo3VibrateData{
		public string vibrateName = "None";
		public float amplitude = 0.3f;
		public float time = 0.2f;
	}
}