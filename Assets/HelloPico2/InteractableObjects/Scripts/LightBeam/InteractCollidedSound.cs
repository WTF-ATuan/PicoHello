using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class InteractCollidedSound : MonoBehaviour, IInteractCollide{
		[Required] [SerializeField] private AudioClip soundEffect;
		[Required] [SerializeField] private AudioSource audioSource;

		public void OnCollide(){
			audioSource.PlayOneShot(soundEffect);
			
		}
	}
}