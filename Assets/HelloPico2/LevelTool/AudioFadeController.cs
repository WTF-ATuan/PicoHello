using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.LevelTool{
	public class AudioFadeController : MonoBehaviour{
		[Required] [SerializeField] private AudioSource audioSource;
		public float fadeTime;

		private float _originVolume;

		private void OnEnable(){
			_originVolume = audioSource.volume;
			audioSource.DOFade(0, fadeTime);
		}

		private void OnDisable(){
			audioSource.DOFade(_originVolume, 0.1f);
		}
	}
}