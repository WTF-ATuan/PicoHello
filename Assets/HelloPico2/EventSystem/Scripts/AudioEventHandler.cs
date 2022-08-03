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
			var allowMultiple = obj.AllowMultiple;
			if(allowMultiple){
				var audioData = dataOverview.FindEventData<MultiAudioData>(audioID);
				var audioClip = audioData.GetItem();
				_audioSource.transform.position = position;
				_audioSource.PlayOneShot(audioClip);
			}
			else{
				var audioData = dataOverview.FindEventData<AudioData>(audioID);
				var audioClip = audioData.clip;
				_audioSource.transform.position = position;
				_audioSource.PlayOneShot(audioClip);
			}
		}

		[Button]
		private void AudioTest(string id){
			var multiAudioData = dataOverview.FindEventData<MultiAudioData>(id);
			var audioClip = multiAudioData.GetItem();
			_audioSource.PlayOneShot(audioClip);
		}

		public void ChangeData(ViewEventDataOverview data){
			dataOverview = data;
		}
	}
}