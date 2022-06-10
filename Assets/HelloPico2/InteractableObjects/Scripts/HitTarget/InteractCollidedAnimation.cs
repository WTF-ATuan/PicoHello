using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	[RequireComponent(typeof(Animator))]
	public class InteractCollidedAnimation : MonoBehaviour, IInteractCollide{
		private Animator _animator;
		[Required] [SerializeField] private AnimationClip animationClip;

		private void Start(){
			_animator = GetComponent<Animator>();
		}

		public void OnCollide(InteractType type, Collider selfCollider){
			_animator.Play(animationClip.name);
		}

		public Action<InteractType, Collider> ColliderEvent{ get; }
	}
}