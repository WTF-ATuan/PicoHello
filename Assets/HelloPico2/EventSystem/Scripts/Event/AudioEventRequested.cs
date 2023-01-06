using UnityEngine;

namespace HelloPico2{
	public class AudioEventRequested{
		public string AudioID{ get; }
		public Vector3 PlayPosition{ get; }

		public bool UsingMultipleAudioClips = false;

		public int ClipsIndex = -1;

		public AudioEventRequested(string audioID, Vector3 playPosition){
			AudioID = audioID;
			PlayPosition = playPosition;
		}
	}
}