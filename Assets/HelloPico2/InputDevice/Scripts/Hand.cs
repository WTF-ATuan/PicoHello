using UnityEngine;

namespace HelloPico2.InputDevice.Scripts{
	public class Hand : MonoBehaviour{
		[SerializeField] public Transform palmTransform;
		
		public Vector3 palmPoint => palmTransform.position;

	}
}