using Project;
using UnityEngine;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.InteractableObjects
{
    public class InteractableBase : MonoBehaviour
    {
		public delegate void InteractableDel(InteractableBase interactable);
		public InteractableDel OnInteractableDisable;
		public void SetUpMoveBehavior(Vector3 dir, float speed, bool useGravity, float gravity)
		{
			var mover = gameObject.AddComponent<HelloPico2.LevelTool.MoveLevelObject>();
			mover.speed = speed;
			mover.dir = dir;			
			mover.useGravity = useGravity;
			mover.gravity = gravity;
		}
		protected virtual void Start()
		{
			
		}
        private void OnDestroy()
        {
			OnInteractableDisable?.Invoke(this);

			if (TryGetComponent<MoveObject>(out var moveObj))
				moveObj.speed = 0;
		}

        public virtual void OnDrop()
		{
			
		}

		public virtual void OnSelect(DeviceInputDetected obj)
		{
			
		}

		public virtual void OnXOrAButton() { }

		public virtual void OnTouchPad(Vector2 axis) { }
	}
}
