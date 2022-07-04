using System;
using DG.Tweening;
using FIMSpace.FLook;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class InteractActionChange : MonoBehaviour, IInteractCollide{
		[SerializeField] private FLookAnimator lookAnimator;

		public void OnCollide(InteractType type, Collider selfCollider){
			if(type == InteractType.Eye){
				EyeTrigger();
			}
		}

		[Button]
		private void EyeTrigger(){
			DOTween.KillAll();
			const float lerpAmount = 0;
			DOTween.To(() => lerpAmount, x => lookAnimator.LookAnimatorAmount = x, 1, 2f);
		}

		public Action<InteractType, Collider> ColliderEvent{ get; }
	}
}