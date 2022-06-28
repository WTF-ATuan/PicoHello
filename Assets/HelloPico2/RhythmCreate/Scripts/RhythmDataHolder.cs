using System;
using System.Collections.Generic;
using Melanchall.DryWetMidi.MusicTheory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.RhythmCreate.Scripts{
	[RequireComponent(typeof(AudioSource))]
	public class RhythmDataHolder : MonoBehaviour{
		private AudioSource _audioSource;
		[SerializeField] private List<LaneByRead> laneList;

		[SerializeField] [Required] [FilePath(ParentFolder = "Assets/Resources", Extensions = "txt , json")]
		private string fileName;

		private void Start(){
			_audioSource = GetComponent<AudioSource>();
			var dotIndex = fileName.IndexOf('.');
			var targetFileName = fileName.Remove(dotIndex);
			var dataText = Resources.Load<TextAsset>(targetFileName);
		}

		private void Play(){
			_audioSource.Play();
		}

		public double GetAudioSourceTime(){
			return (double)_audioSource.timeSamples / _audioSource.clip.frequency;
		}
	}
}