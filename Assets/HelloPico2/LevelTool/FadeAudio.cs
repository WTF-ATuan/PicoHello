using DG.Tweening;
using HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene;
using UnityEngine;

namespace HelloPico2.LevelTool{
	[RequireComponent(typeof(AudioSource))]
	public class FadeAudio : MonoBehaviour{
		private AudioSource _audioSource;

		private void Awake(){
			_audioSource = GetComponent<AudioSource>();
		}

		public void Handle(CrossEvent crossEvent){
			var audioInterrupted = (AudioInterrupted)crossEvent;
			var fadeTime = audioInterrupted.fadeTime;
			_audioSource.DOFade(0, fadeTime)
					.OnComplete(() => _audioSource.DOFade(1, 0));
		}
	}
}