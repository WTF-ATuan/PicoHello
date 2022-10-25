using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class WaveSpawnCycle : MonoBehaviour{
		[ReadOnly] public int waveCount = 1;
		[ChildGameObjectsOnly] public List<SpawnLoopAnimator> spawners;
		private CancellationToken _token;


		private void OnEnable(){
			_token = new CancellationToken(false);
			Loop();
		}

		private void OnDisable(){
			_token = new CancellationToken(true);
		}

		private async void Loop(){
			await Task.Delay(Mathf.FloorToInt(GetWaveDurationTime() * 1000), _token);
			if(_token.IsCancellationRequested) return;
			SetSpawnerLoopState(false);
			await Task.Delay(Mathf.FloorToInt(GetWaveBreakTime() * 1000), _token);
			if(_token.IsCancellationRequested) return;
			waveCount++;
			SetSpawnerLoopState(true);
			Loop();
		}

		private float GetWaveDurationTime(){
			return 15 + 5 * waveCount;
		}

		private void SetSpawnerLoopState(bool isStart){
			var spawnCount = waveCount > spawners.Count ? spawners.Count : waveCount;
			for(var i = 0; i < spawnCount; i++){
				var loopAnimator = spawners[i];
				if(isStart){
					loopAnimator.StartLoop();
				}
				else{
					loopAnimator.StopLoop();
				}
			}
		}

		private float GetWaveBreakTime(){
			return 5;
		}
	}
}