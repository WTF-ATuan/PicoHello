using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace HelloPico2.InteractableObjects{
	public class HitTargetRock : HitTargetBase{
		[BoxGroup("Hit Count")] public int beamHitCount = 1;
		[BoxGroup("Hit Count")] public int ballHitCount = 3;
		[SerializeField] private float _DestroyDelayDuration = 3;
		[SerializeField] private bool _GenerateHitVFX = true;
		[SerializeField] private string _HitEffectID = "";
		[SerializeField] private bool _UseDestroySFX = false;
		[ShowIf("_UseDestroySFX")][SerializeField] private int[] _NormalAudioIndex;
		[ShowIf("_UseDestroySFX")][SerializeField] private int[] _DestroyAudioIndex;
		[SerializeField] private bool _DestroyAfterHit = true;

		public UltEvents.UltEvent WhenCollideWithEnergyBall;

		private void OnEnable(){
			OnEnergyBallInteract += x => OnInteract(x, InteractType.EnergyBall);
			OnShieldInteract += DestroyBullet;
			OnWhipInteract += x => OnInteract(x, InteractType.Whip);
			OnBeamInteract += x => OnInteract(x, InteractType.Beam);
            OnEnergyInteract += x => OnInteract(x, InteractType.Energy);
		}

		private void OnDisable(){
			OnEnergyBallInteract -= x => OnInteract(x, InteractType.EnergyBall);
			OnShieldInteract -= DestroyBullet;
			OnWhipInteract -= x => OnInteract(x, InteractType.Whip);
			OnBeamInteract -= x => OnInteract(x, InteractType.Beam);
            OnEnergyInteract -= x => OnInteract(x, InteractType.Energy);
		}

		private void OnInteract(Collider selfCollider, InteractType type){
			print(selfCollider.name);
			var isBeam = type == InteractType.Beam || type == InteractType.Whip;
			if(isBeam){
				beamHitCount -= 1;
				if(beamHitCount < 1){
					DestroyBullet(selfCollider);
				}
				else{
					BulletReact(selfCollider);
				}
			}
			else{
				ballHitCount -= 1;
				if(ballHitCount < 1){
					DestroyBullet(selfCollider);
				}
				else{
					BulletReact(selfCollider);
				}
			}
		}

		private void BulletReact(Collider selfCollider){
			WhenCollideWithEnergyBall?.Invoke();

			GenerateHitVFX();

            if (_UseDestroySFX)
				PlayAudio(_NormalAudioIndex);
			else
				PlayRandomAudio();
		}

		private void DestroyBullet(Collider selfCollider){
			if(_UsephPushBackFeedback) PushBackFeedback(selfCollider);

			WhenCollideUlt?.Invoke();

			GenerateHitVFX();

            if (_UseDestroySFX)
				PlayAudio(_DestroyAudioIndex);
			else
				PlayRandomAudio();

			if (TryGetComponent<MoveObject>(out var moveObj))
				moveObj.speed = 0;
			//transform.DOKill();
			WhenCollide?.Invoke();
			WhenCollideUlt?.Invoke();

			if(_DestroyAfterHit)
				Destroy(gameObject, _DestroyDelayDuration);
		}
		private void GenerateHitVFX() {
			if (!_GenerateHitVFX) return;

            Project.EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
                _HitEffectID,
                false,
                _DestroyDelayDuration,
                transform.position));
        }
		protected override void PushBackFeedback(Collider hitCol){
			base.PushBackFeedback(hitCol);

			var targetPos = transform.position + hitCol.transform.forward * _PushBackDist;
			transform.DOKill();
			transform.DOMove(targetPos, _PushBackDuration).SetEase(_PushBackEasingCureve);
		}
	}
}