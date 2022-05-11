using Project;
using UnityEngine;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.InteractableObjects
{
    public class InteractableBase : MonoBehaviour
    {
		public delegate void InteractableDel(InteractableBase interactable);
		public InteractableDel OnInteractableDisable;

		protected virtual void Start()
		{
			
		}
        private void OnDestroy()
        {
			OnInteractableDisable?.Invoke(this);
        }

        public virtual void OnDrop()
		{
			var meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material.color = Color.white;
		}

		public virtual void OnSelect(DeviceInputDetected obj)
		{
			var meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material.color = Color.red;
		}

		public virtual void OnXOrAButton() { }

		public virtual void OnTouchPad(Vector2 axis) { }
	}
}
