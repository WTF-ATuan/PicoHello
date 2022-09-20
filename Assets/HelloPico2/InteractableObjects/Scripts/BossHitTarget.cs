using System;
using System.Collections.Generic;
using System.Linq;
using Game.Project;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HelloPico2.InteractableObjects.Scripts{
	public class BossHitTarget : MonoBehaviour{
		[ReadOnly] public float elapsedTime = 0;
		[Required] public ParticleSystem defaultEffect;
		[Required] public string defaultAudioName;
		public List<HitEffect> effectSettings = new List<HitEffect>();
		private ParticleSystem _currentEffect;

		[FoldoutGroup("Call back")] public UnityEvent onBossHit;

		[FoldoutGroup("Call back")] public int hitAmount = 3;
		[FoldoutGroup("Call back")] public UnityEvent onBossHitMultiple;
		private int _hitCount = 0;

		private void OnEnable(){
			ChangeEffect(defaultEffect);
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
				ChangeEffect(effect.effect);
				effect.isTrigger = true;
			}
		}

		private void ChangeEffect(ParticleSystem effect){
			if(_currentEffect){
				var currentEffectObject = _currentEffect.gameObject;
				Destroy(currentEffectObject);
			}

			var currentTransform = transform;
			var effectObject = Instantiate(effect, currentTransform.position, Quaternion.identity, currentTransform);
			_currentEffect = effectObject;
		}

		private void OnTriggerEnter(Collider other){
			var collisionPoint = other.ClosestPoint(transform.position);
			if(other.gameObject.layer != LayerMask.NameToLayer("sampleball")) return;
			_currentEffect.transform.position = collisionPoint;
			_currentEffect.Play();
			var audioEventRequested = new AudioEventRequested(defaultAudioName, collisionPoint){
				UsingMultipleAudioClips = true
			};
			EventBus.Post(audioEventRequested);
			TriggerCallback();
		}

		private void TriggerCallback(){
			onBossHit?.Invoke();
			_hitCount++;
			if(_hitCount > hitAmount){
				onBossHitMultiple?.Invoke();
				_hitCount = 0;
			}
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
			[Required] public string audioDataName;
			public bool isTrigger;
		}
	}
}