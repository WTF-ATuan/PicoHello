using System;
using System.Collections.Generic;
using System.Linq;
using Game.Project;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects.Scripts{
	public class BossHitTarget : MonoBehaviour, IInteractCollide{
		[ReadOnly] public float elapsedTime = 0;
		[Required] public ParticleSystem defaultEffect;
		[Required] public string defaultAudioName;
		public List<HitEffect> effectSettings = new List<HitEffect>();
		private ParticleSystem _currentEffect;
		private ColdDownTimer _timer;

		public Action<InteractType, Vector3> _OnHitEvent;

		public Action<InteractType, Collider> ColliderEvent{ get; }

		private void OnEnable(){
			ChangeEffect(defaultEffect);
			_timer = new ColdDownTimer(0.5f);

			_OnHitEvent += GetComponentInParent<BossVisualReaction>().OnHit;
		}

		private void OnDisable(){
			elapsedTime = 0;
			foreach(var effect in effectSettings){
				effect.isTrigger = false;
			}

			_OnHitEvent -= GetComponentInParent<BossVisualReaction>().OnHit;
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
			if(!other.gameObject.CompareTag("PlayerWeapon")) return;
			ReceiveDamage(collisionPoint);
		}
		public void ReceiveDamage(Vector3 collisionPoint) { 
			if(!_timer.CanInvoke()) return;
			_currentEffect.transform.position = collisionPoint;
			_currentEffect.Play();
			var audioEventRequested = new AudioEventRequested(defaultAudioName, collisionPoint){
				UsingMultipleAudioClips = true
			};
			EventBus.Post(audioEventRequested);
			_timer.Reset();

			_OnHitEvent?.Invoke(InteractType.EnergyBall, collisionPoint);
		}

		[Button]
		private void TestTrigger(Vector3 offset){
			var triggerPosition = transform.position + offset;
			_currentEffect.transform.position = triggerPosition;
			_currentEffect.Play();
		}

		public void OnCollide(InteractType type, Collider selfCollider){
			if(!_timer.CanInvoke()) return;
			_OnHitEvent?.Invoke(type, selfCollider.transform.position);
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