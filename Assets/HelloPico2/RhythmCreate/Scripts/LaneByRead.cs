using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.RhythmCreate.Scripts{
	public class LaneByRead : MonoBehaviour{
		[SerializeField] [FilePath] [Required] private string fileName;

		private int _randomPrefabCountLimit;
		public GameObject[] notePrefab;
		private readonly List<Note> _notes = new List<Note>();
		public List<double> timeStamps = new List<double>();

		private int _spawnIndex = 0;
		private int _inputIndex = 0;

		// Start is called before the first frame update
		private void Start(){
			_randomPrefabCountLimit = notePrefab.Length;
			SetTimeStamps();
		}

		private void SetTimeStamps(){
			var reader = new StreamReader(fileName);
			Debug.Log($"{reader.EndOfStream}");
			while(!reader.EndOfStream){
				var readData = reader.ReadLine();
				if(readData == null) continue;
				var doubleValue = double.Parse(readData);
				timeStamps.Add(doubleValue);
			}
		}

		private void Update(){
			if(_spawnIndex < timeStamps.Count){
				if(SongManager.GetAudioSourceTime() >= timeStamps[_spawnIndex] - SongManager.Instance.noteTime){
					var note = Instantiate(notePrefab[UnityEngine.Random.Range(0, _randomPrefabCountLimit)], transform);
					_notes.Add(note.GetComponent<Note>());
					note.GetComponent<Note>().assignedTime = (float)timeStamps[_spawnIndex];
					_spawnIndex++;
				}
			}

			if(_inputIndex < timeStamps.Count){
				var timeStamp = timeStamps[_inputIndex];
				var marginOfError = SongManager.Instance.marginOfError;
				var audioTime = SongManager.GetAudioSourceTime() -
								(SongManager.Instance.inputDelayInMilliseconds / 1000.0);

				if(Math.Abs(audioTime - timeStamp) < marginOfError){
					Destroy(_notes[_inputIndex].gameObject);
					_inputIndex++;
				}

				if(timeStamp + marginOfError <= audioTime){
					_inputIndex++;
				}
			}
		}
	}
}