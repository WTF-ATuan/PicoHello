using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace HelloPico2.InteractableObjects
{
	public class HitTargetCage : HitTargetBase
	{
		[BoxGroup("Hit Count")] public int beamHitCount = 1;
		[BoxGroup("Hit Count")] public int ballHitCount = 3;
		[SerializeField] private float _DestroyDelayDuration = 3;
		[SerializeField] private string _HitEffectID = "";

		public UltEvents.UltEvent WhenCollideWithEnergyBall;

		private void OnEnable()
		{
			OnEnergyBallInteract += x => OnInteract(x, InteractType.EnergyBall);
			OnShieldInteract += DestroyBullet;
			OnWhipInteract += x => OnInteract(x, InteractType.Whip);
			OnBeamInteract += x => OnInteract(x, InteractType.Beam);
		}

		private void OnDisable()
		{
			OnEnergyBallInteract -= x => OnInteract(x, InteractType.EnergyBall);
			OnShieldInteract -= DestroyBullet;
			OnWhipInteract -= x => OnInteract(x, InteractType.Whip);
			OnBeamInteract -= x => OnInteract(x, InteractType.Beam);
		}

		private void OnInteract(Collider selfCollider, InteractType type)
		{
			var isBeam = type == InteractType.Beam || type == InteractType.Whip;
			if (isBeam)
			{
				beamHitCount -= 1;
				if (beamHitCount < 1)
				{
					DestroyBullet(selfCollider);
				}
				else
				{
					BulletReact(selfCollider);
				}
			}
			else
			{
				ballHitCount -= 1;
				if (ballHitCount < 1)
				{
					DestroyBullet(selfCollider);
				}
				else
				{
					BulletReact(selfCollider);
				}
			}
		}

		private void BulletReact(Collider selfCollider)
		{
			WhenCollideWithEnergyBall?.Invoke();

			Project.EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
				_HitEffectID,
				false,
				_DestroyDelayDuration,
				transform.position));

			PlayRandomAudio();
		}

		private void DestroyBullet(Collider selfCollider)
		{
			if (_UsephPushBackFeedback) PushBackFeedback(selfCollider);

			WhenCollideUlt?.Invoke();

			Project.EventBus.Post<VFXEventRequested, ParticleSystem>(new VFXEventRequested(
				_HitEffectID,
				false,
				_DestroyDelayDuration,
				transform.position));

			PlayRandomAudio();

			if (TryGetComponent<MoveObject>(out var moveObj))
				moveObj.speed = 0;
			//transform.DOKill();
			WhenCollide?.Invoke();
			WhenCollideUlt?.Invoke();
			Destroy(gameObject, _DestroyDelayDuration);

			// Facing Player
			FacingPlayer();
		}
		private void FacingPlayer() {
			var dir = (HelloPico2.Singleton.GameManagerHelloPico.Instance._Player.transform.position - transform.position).normalized;
			transform.DORotateQuaternion(Quaternion.LookRotation(dir), .5f);
		}
		protected override void PushBackFeedback(Collider hitCol)
		{
			base.PushBackFeedback(hitCol);

			var targetPos = transform.position + hitCol.transform.forward * _PushBackDist;
			transform.DOKill();
			transform.DOMove(targetPos, _PushBackDuration).SetEase(_PushBackEasingCureve);
		}
	}
}