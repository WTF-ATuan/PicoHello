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
		[SerializeField] private AnimationCurve difficultyCurve = AnimationCurve.Linear(0, 0, 1, 1);


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
			return 10 * waveCount;
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
			activeSpawner.ForEach(x => {
				x.duringMinMax.x *= 0.8f;
				x.duringMinMax.y *= 0.8f;
			});
		}

		private float GetWaveBreakTime(){
			//TODO : 依照目前Wave 數調整Delay時間 
			return 5;
		}
	}
}