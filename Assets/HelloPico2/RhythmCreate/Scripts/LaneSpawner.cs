using System.Collections.Generic;
using Melanchall.DryWetMidi.MusicTheory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.RhythmCreate.Scripts{
	public class LaneSpawner : MonoBehaviour{
		public GameObject[] prefabList;
		[SerializeField] [EnumPaging] private NoteName laneNote;
		private int _randomPrefabCountLimit;
		private readonly List<GameObject> _objectPool = new List<GameObject>();
		[ReadOnly] public List<double> timeStamps = new List<double>();

		private int _spawnIndex = 0;
		private IRhythmTime _rhythmTime;

		public NoteName GetLaneNote(){
			return laneNote;
		}

		public void Init(IRhythmTime rhythmTime, List<double> timeStampList){
			_rhythmTime = rhythmTime;
			_randomPrefabCountLimit = prefabList.Length;
			timeStamps = timeStampList;
		}


		private void Update(){
			Spawn();
		}

		private void Spawn(){
			if(_spawnIndex < timeStamps.Count){
				if(_rhythmTime.GetAudioSourceTime() >= timeStamps[_spawnIndex]){
					var instance = Instantiate(prefabList[Random.Range(0, _randomPrefabCountLimit)], transform);
					_objectPool.Add(instance);
					_spawnIndex++;
				}
			}
		}
	}
}