using UnityEngine;

namespace HelloPico2{
	public class AudioEventPosted{
		public string AudioID{ get; }
		public Vector3 PlayPosition{ get; }

		public AudioEventPosted(string audioID, Vector3 playPosition){
			AudioID = audioID;
			PlayPosition = playPosition;
		}
	}
}