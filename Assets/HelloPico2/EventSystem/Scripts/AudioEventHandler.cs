using System;
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
			_audioSource.priority = 999;
		}

		private void OnAudioEventPosted(AudioEventRequested obj){
			var audioID = obj.AudioID;
			var position = obj.PlayPosition;
			var usingMultiple = obj.UsingMultipleAudioClips;
			if(usingMultiple){
				var audioData = dataOverview.FindEventData<MultiAudioData>(audioID);
				if(audioData == null){
					throw new Exception($"not found {audioID} in List please check {dataOverview}");
				}

				var audioClip = audioData.GetAudio();
				_audioSource.transform.position = position;
				_audioSource.PlayOneShot(audioClip);
			}
			else{
				var audioData = dataOverview.FindEventData<AudioData>(audioID);
				if(audioData == null){
					throw new Exception($"not found {audioID} in List please check {dataOverview}");
				}

				var audioClip = audioData.clip;
				_audioSource.transform.position = position;
				_audioSource.PlayOneShot(audioClip);
			}
		}

		[Button]
		private void AudioTest(string id){
			var multiAudioData = dataOverview.FindEventData<MultiAudioData>(id);
			var audioClip = multiAudioData.GetAudio();
			_audioSource.PlayOneShot(audioClip);
		}

		public void ChangeData(ViewEventDataOverview data){
			dataOverview = data;
		}
	}
}