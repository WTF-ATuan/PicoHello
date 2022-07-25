using UnityEngine;

namespace HelloPico2.PlayerController.Arm{
	public class HandIK : MonoBehaviour{
		[SerializeField] private GameObject followObject;
		[SerializeField] private float followSpeed = 30f;
		[SerializeField] private float rotateSpeed = 100f;
		[SerializeField] private Vector3 positionOffset;
		[SerializeField] private Vector3 rotationOffset;

		private Transform _followTarget;
		private Rigidbody _rigidbody;

		private void Start(){
			_followTarget = followObject.transform;
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			_rigidbody.mass = 20f;

			_rigidbody.position = _followTarget.position;
			_rigidbody.rotation = _followTarget.rotation;
		}

		private void Update(){
			var followPositionOffset = _followTarget.TransformPoint(positionOffset);
			var distance = Vector3.Distance(followPositionOffset, transform.position);
			_rigidbody.velocity = (followPositionOffset - transform.position).normalized * (followSpeed * distance);

			var followRotationOffset = _followTarget.rotation * Quaternion.Euler(rotationOffset);
			var quaternion = followRotationOffset * Quaternion.Inverse(_rigidbody.rotation);
			quaternion.ToAngleAxis(out var angle, out var axis);
			_rigidbody.angularVelocity = axis * (angle * Mathf.Deg2Rad * rotateSpeed);
		}
	}
}