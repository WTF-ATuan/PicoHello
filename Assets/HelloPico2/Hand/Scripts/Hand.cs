using UnityEngine;

namespace HelloPico2.Hand.Scripts{
	public class Hand : MonoBehaviour{
		[SerializeField] public Transform palmTransform;
		
		public Vector3 palmPoint => palmTransform.position;
	}
}