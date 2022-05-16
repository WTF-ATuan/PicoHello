using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	[RequireComponent(typeof(Animator))]
	public class BeamCollidedAnimation : MonoBehaviour, IBeamCollide{
		private Animator _animator;
		[Required] [SerializeField] private AnimationClip animationClip;

		private void Start(){
			_animator = GetComponent<Animator>();
		}

		public void OnCollide(){
			_animator.Play(animationClip.name);
		}
	}
}