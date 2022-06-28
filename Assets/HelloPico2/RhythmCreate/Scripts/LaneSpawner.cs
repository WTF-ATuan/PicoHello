using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HelloPico2.RhythmCreate.Scripts{
	public class LaneSpawner : MonoBehaviour{
		[SerializeField] [FilePath] [Required] private string fileName;
		public GameObject[] prefabList;
		private int _randomPrefabCountLimit;
		private readonly List<GameObject> _objectPool = new List<GameObject>();
		[ReadOnly] public List<double> timeStamps = new List<double>();

		private int _spawnIndex = 0;
		private IRhythmTime _rhythmTime;
		
		public void Init(IRhythmTime rhythmTime){
			_rhythmTime = rhythmTime;
			_randomPrefabCountLimit = prefabList.Length;
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