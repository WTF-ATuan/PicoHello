using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
    public class SpawnLoopAnimator : MonoBehaviour{
        [SerializeField] [EnumToggleButtons] private SpawnLoopType loopType;

        [SerializeField] [ShowIf("IsDuringLoopType")]
        private float during = 5;

        [SerializeField] private float delayOpenTime = 1f;

        private CancellationToken _token;

        private void OnEnable(){
            _token = new CancellationToken(false);
            Loop();
        }

        private void OnDestroy(){
            _token = new CancellationToken(true);
        }

        private async void Loop(){
            await Task.Delay(Mathf.FloorToInt(during * 1000));
            if(_token.IsCancellationRequested) return;
            gameObject.SetActive(false);
            await Task.Delay(Mathf.FloorToInt(delayOpenTime * 1000));
            if(_token.IsCancellationRequested) return;
            gameObject.SetActive(true);
        }

        private bool IsDuringLoopType() => loopType == SpawnLoopType.During;
    }

    public enum SpawnLoopType{
        During,
        FollowSpawner,
    }
}