using UnityEngine;

namespace HelloPico2.Hand.Scripts.Event{
	public interface IGrip{
		void OnGrip(float gripValue);
	}

	public interface ITrigger{
		void OnTrigger(float triggerValue);
	}

	public interface ITouchPad{
		void OnTouchPad(Vector2 padAxis);
	}

	public interface IPrimaryButton{
		void OnPrimaryButtonClick();
	}

	public interface ISecondaryButton{
		void OnSecondaryButtonClick();
	}
}