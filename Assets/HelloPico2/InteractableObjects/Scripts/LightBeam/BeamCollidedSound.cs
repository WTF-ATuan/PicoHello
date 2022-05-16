using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class BeamCollidedSound : MonoBehaviour, IBeamCollide{
		[Required] [SerializeField] private AudioClip soundEffect;
		[Required] [SerializeField] private AudioSource audioSource;

		public void OnCollide(){
			audioSource.PlayOneShot(soundEffect);
		}
	}
}