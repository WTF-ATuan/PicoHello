using UnityEngine;

namespace HelloPico2.Hand.Scripts{
	public class Hand : MonoBehaviour{
		[SerializeField] private Transform palmTransform;
		
		public Vector3 palmPoint => palmTransform.position;
	}
}