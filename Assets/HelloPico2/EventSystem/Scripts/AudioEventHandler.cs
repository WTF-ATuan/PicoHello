using Project;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2{
	[RequireComponent(typeof(AudioSource))]
	public class AudioEventHandler : MonoBehaviour{
		[Required] [SerializeField] private ViewEventDataOverview dataOverview;

		private AudioSource _audioSource;

		private void Start(){
			EventBus.Subscribe<AudioEventRequested>(OnAudioEventPosted);
			_audioSource = GetComponent<AudioSource>();
		}

		private void OnAudioEventPosted(AudioEventRequested obj){
			var audioID = obj.AudioID;
			var position = obj.PlayPosition;
			var audioData = dataOverview.FindEventData<AudioData>(audioID);
			var audioClip = audioData.clip;
			_audioSource.transform.position = position;
			_audioSource.PlayOneShot(audioClip);
		}
	}
}