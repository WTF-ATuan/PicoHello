using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class SpawnLoopAnimator : MonoBehaviour{
		[SerializeField] [EnumToggleButtons] private SpawnLoopType loopType;

		[SerializeField] [ShowIf("IsDuringLoopType")]
		private float during = 5;

		[SerializeField] private float delayOpenTime = 1f;
		
		private void OnEnable(){
			Loop();
		}

		private async void Loop(){
			await Task.Delay(Mathf.FloorToInt(during * 1000));
			gameObject.SetActive(false);
			await Task.Delay(Mathf.FloorToInt(delayOpenTime * 1000));
			gameObject.SetActive(true);
		}

		private bool IsDuringLoopType() => loopType == SpawnLoopType.During;
	}

	public enum SpawnLoopType{
		During,
		FollowSpawner,
	}
}