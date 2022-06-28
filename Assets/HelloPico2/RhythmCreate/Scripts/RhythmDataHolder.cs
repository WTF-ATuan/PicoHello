﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.RhythmCreate.Scripts{
	[RequireComponent(typeof(AudioSource))]
	public class RhythmDataHolder : MonoBehaviour, IRhythmTime{
		private AudioSource _audioSource;
		[SerializeField] private List<LaneSpawner> laneList;

		[SerializeField] [Required] [FilePath(ParentFolder = "Assets/Resources", Extensions = "txt , json")]
		private string fileName;

		private RhythmDataReader _dataReader;

		private void Start(){
			_audioSource = GetComponent<AudioSource>();
			ReadMidiFile();
			InitLaneSpawner();
		}

		private void InitLaneSpawner(){
			foreach(var laneSpawner in laneList){
				var stampDictionary = _dataReader.StampDictionary;
				var containsKey = stampDictionary.ContainsKey(laneSpawner.GetLaneNote());
				if(!containsKey) throw new Exception($"{laneSpawner.GetLaneNote()} is not found in {fileName}");
				var timeStamps = stampDictionary[laneSpawner.GetLaneNote()];
				laneSpawner.Init(this, timeStamps);
			}
		}

		private void ReadMidiFile(){
			var dotIndex = fileName.IndexOf('.');
			var targetFileName = fileName.Remove(dotIndex);
			var dataText = Resources.Load<TextAsset>(targetFileName);
			_dataReader = new RhythmDataReader(dataText);
		}

		private void Play(){
			_audioSource.Play();
		}

		public double GetAudioSourceTime(){
			if(!_audioSource.clip) throw new Exception("Clip is Not Assign");
			return (double)_audioSource.timeSamples / _audioSource.clip.frequency;
		}
	}

	public interface IRhythmTime{
		public double GetAudioSourceTime();
	}
}