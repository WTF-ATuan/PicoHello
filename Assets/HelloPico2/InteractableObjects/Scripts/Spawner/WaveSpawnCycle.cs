using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class WaveSpawnCycle : MonoBehaviour{
		[ReadOnly] public int waveCount = 1;
		[ChildGameObjectsOnly] public List<RotateSpawner> spawners;
		[SerializeField] [MinMaxSlider(1, 10 , true)] private Vector2 delayOpenRange = new Vector2(5,10);
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
			await Task.Delay(Mathf.FloorToInt(GetSpawnerMaxDuring() * 1000));
			if(_token.IsCancellationRequested) return;
			SetSpawnerActive(false);
			await Task.Delay(Mathf.FloorToInt(GetDelayOpenTime() * 1000));
			if(_token.IsCancellationRequested) return;
			SetSpawnerActive(true);
		}

		private float GetSpawnerMaxDuring(){
			var max = spawners.Max(x => x.duringMinMax.y);
			return max;
		}

		private void SetSpawnerActive(bool active){
			//TODO : 依照目前Wave 數量調整Active 數量 
			spawners.ForEach(x => x.enabled = active);
		}

		private float GetDelayOpenTime(){
			//TODO : 依照目前Wave 數調整Delay時間 
			var delayOpenTime = Random.Range(delayOpenRange.x, delayOpenRange.y);
			return delayOpenTime;
		}
	}
}