using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects.Scripts{
	public class BossHitTarget : MonoBehaviour{
		[ReadOnly] public float elapsedTime = 0;
		[Required] public ParticleSystem defaultEffect;
		[Required] public AudioClip defaultAudioClip;
		public List<HitEffect> effectSettings = new List<HitEffect>();

		private ParticleSystem _currentEffect;
		private AudioSource _currentAudio;


		private void OnEnable(){
			ChangeEffect(defaultEffect , defaultAudioClip);
		}

		private void OnDisable(){
			elapsedTime = 0;
			foreach(var effect in effectSettings){
				effect.isTrigger = false;
			}
		}

		private void LateUpdate(){
			elapsedTime += Time.fixedDeltaTime;
			foreach(var effect in effectSettings
							.Where(effect => elapsedTime > effect.triggerTime)
							.Where(effect => !effect.isTrigger)){
				ChangeEffect(effect.effect , effect.audioClip);
				effect.isTrigger = true;
			}
		}

		private void ChangeEffect(ParticleSystem effect, AudioClip audioClip){
			if(_currentEffect){
				var currentEffectObject = _currentEffect.gameObject;
				Destroy(currentEffectObject);
			}

			var currentTransform = transform;
			var effectObject = Instantiate(effect, currentTransform.position, Quaternion.identity, currentTransform);
			_currentEffect = effectObject;
			_currentAudio = effectObject.gameObject.AddComponent<AudioSource>();
			_currentAudio.playOnAwake = false;
			_currentAudio.clip = audioClip;
		}

		private void OnTriggerEnter(Collider other){
			var collisionPoint = other.ClosestPoint(transform.position);
			_currentEffect.transform.position = collisionPoint;
			_currentEffect.Play();
			_currentAudio.Play();
		}

		[Button]
		private void TestTrigger(Vector3 offset){
			var triggerPosition = transform.position + offset;
			_currentEffect.transform.position = triggerPosition;
			_currentEffect.Play();
		}

		[Serializable]
		public class HitEffect{
			public float triggerTime = 20;
			[Required] public ParticleSystem effect;
			[Required] public AudioClip audioClip;
			public bool isTrigger;
		}
	}
}