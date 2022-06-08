using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public interface IInteractCollide{
		void OnCollide(InteractType type, Collider selfCollider);
	}
}