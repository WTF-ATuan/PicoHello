using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
	public class WaveSpawnCycle : MonoBehaviour{
		[ReadOnly] public int waveCount = 1;
		[ChildGameObjectsOnly] public List<RotateSpawner> spawners;
		[SerializeField] private AnimationCurve waveDurationCurve = AnimationCurve.Linear(1, 10, 2, 20);
		[SerializeField] private AnimationCurve spawnDurationCurve = AnimationCurve.Linear(1, 8, 2, 5);
		[SerializeField] private AnimationCurve targetMovingSpeedCurve = AnimationCurve.Linear(1, 5, 2, 3.5f);
		private CancellationToken _token;


		private void OnEnable(){
			_token = new CancellationToken(false);
			Loop();
		}

		private void OnDisable(){
			_token = new CancellationToken(true);
		}

		private async void Loop(){
			var waveDurationTime = GetWaveDurationTime();
			await Task.Delay(Mathf.FloorToInt(waveDurationTime * 1000));
			if(_token.IsCancellationRequested) return;
			SetSpawnerActive(false);
			var breakTime = GetWaveBreakTime();
			await Task.Delay(Mathf.FloorToInt(breakTime * 1000));
			if(_token.IsCancellationRequested) return;
			SetSpawnerActive(true);

			if(_token.IsCancellationRequested) return;
			ModifySpawnerSpeed();
			waveCount++;
			Loop();
		}

		private float GetWaveDurationTime(){
			//TODO : 依照目前Wave 數量調整Wave 時間 
			var index = (waveDurationCurve.length > waveCount ? waveCount : waveDurationCurve.length) - 1;
			var key = waveDurationCurve[index];
			var duration = key.value;
			return duration;
		}

		private void SetSpawnerActive(bool active){
			//TODO : 依照目前Wave 數量調整Active 數量 
			if(waveCount > spawners.Count) return;
			for(var i = 0; i < waveCount; i++){
				spawners[i].gameObject.SetActive(active);
			}
		}

		private void ModifySpawnerSpeed(){
			var activeSpawner = spawners.FindAll(x => x.gameObject.activeSelf);
			var index = (spawnDurationCurve.length > waveCount ? waveCount : waveDurationCurve.length) - 1;
			var key = spawnDurationCurve[index];
			var duration = key.value;
			activeSpawner.ForEach(x => {
				x.duringMinMax.x = duration - 1;
				x.duringMinMax.y = duration;
			});
		}

		private float GetWaveBreakTime(){
			//TODO : 依照目前Wave 數調整Delay時間 
			return 5;
		}
	}
}