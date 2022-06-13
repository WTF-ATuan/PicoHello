using DG.Tweening;
using HelloPico2.InputDevice.Scripts;
using HelloPico2.InteractableObjects;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm{
	[RequireComponent(typeof(ArmLogic))]
	public class ShieldBehavior : WeaponBehavior{
		[SerializeField] private Vector2 _ScaleRange;
		[SerializeField] private float _ScalingDuration;

		[Header("Special Skill")] [SerializeField]
		private float speedLimit;

		[SerializeField] private float speedDuring;
		[SerializeField] private float range;


		private float timer;
		private Collider _shieldCollider;

		GameObject shield{ get; set; }
		ArmLogic armLogic{ get; set; }

		public override void Activate(ArmLogic Logic, ArmData data, GameObject shieldObj, Vector3 fromScale){
			armLogic = Logic;
			shield = shieldObj;
			_shieldCollider = shieldObj.GetComponentInChildren<Collider>();
			UpdateShieldScale(data);
			armLogic.OnEnergyChanged += UpdateShieldScale;
			armLogic.OnUpdateInput += DetectDeviceSpeed;

			base.Activate(Logic, data, shieldObj, fromScale);
		}

		public override void Deactivate(GameObject obj){
			if(armLogic != null){
				armLogic.OnEnergyChanged -= UpdateShieldScale;
				armLogic.OnUpdateInput -= DetectDeviceSpeed;
			}

			shield.transform.DOScale(new Vector3(0, 0, shield.transform.localScale.z), _DeactiveDuration).OnComplete(
				() => {
					obj.SetActive(false);
					_FinishedDeactivate?.Invoke();
				});

			base.Deactivate(obj);
		}

		private void OnDisable(){
			//Deactivate();
			StopAllCoroutines();
		}

		private void UpdateShieldScale(ArmData data){
			var targetScale = Mathf.Lerp(_ScaleRange.x, _ScaleRange.y, data.Energy / data.MaxEnergy);
			if(data.Energy == 0) targetScale = 0;
			shield.transform.DOScale(new Vector3(targetScale, targetScale, shield.transform.localScale.z),
				_ScalingDuration);
		}

		private void DetectDeviceSpeed(DeviceInputDetected inputDetected){
			var selectorSpeed = inputDetected.Selector.Speed;
			if(selectorSpeed > speedLimit){
				timer += Time.fixedDeltaTime;
				if(timer > speedDuring){
					// ReSharper disable once Unity.PreferNonAllocApi
					var raycastHits = Physics.SphereCastAll(shield.transform.position, range, shield.transform.forward);
					foreach(var hit in raycastHits){
						var interactablePower = hit.transform.GetComponent<InteractablePower>();
						interactablePower?.OnSelect(inputDetected);
					}

					timer = 0;
				}
			}
		}

		private void OnDrawGizmos(){
			if(_shieldCollider == null) return;

			var bounds = _shieldCollider.bounds;
			var boundsCenter = bounds.center;
			var boundsSize = bounds.size;
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(boundsCenter, boundsSize);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(shield.transform.position, range);
		}
	}
}