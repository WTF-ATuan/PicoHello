using System;
using UnityEngine;

namespace HelloPico2.Trajectory{
	public class MoveWithDirection : MonoBehaviour{
		[SerializeField] private Vector3 direction;
		[SerializeField] private float speed;

		private void Start(){
			Destroy(gameObject, 5f);
		}

		private void Update(){
			transform.position += direction * speed;
		}
	}
}