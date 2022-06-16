using DG.Tweening;
using HelloPico2.InputDevice.Scripts;
using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm{
	[RequireComponent(typeof(ArmLogic))]
	public class ShieldBehavior : WeaponBehavior{		
		[FoldoutGroup("Shield Scale")][SerializeField] private Vector2 _ScaleRange;
		[FoldoutGroup("Shield Scale")][SerializeField] private float _ScalingDuration;
		[FoldoutGroup("Interaction")][SerializeField] private int _SpentEnergyWhenCollide = 30;
		[FoldoutGroup("Special Skill")][SerializeField] private float _AbsorbCoolDownDuration = .5f;
		[FoldoutGroup("Special Skill")][SerializeField] private float _AbsorbInterpolateDuration = 0.1f;
		[FoldoutGroup("Special Skill")][SerializeField] private float range;
		[FoldoutGroup("Velocity Detection Settings")][SerializeField] private float speedLimit;
		[FoldoutGroup("Velocity Detection Settings")][SerializeField] private float speedDuring;

		
		private float timer;
		private Collider _shieldCollider;
		private ShieldController _shieldController;

		GameObject shield{ get; set; }
		ArmLogic armLogic{ get; set; }		
		Game.Project.ColdDownTimer absorbCooldown { get; set; }
		Coroutine absorbProcess { get; set; }
        private void Start()
        {
			absorbCooldown = new Game.Project.ColdDownTimer(_AbsorbCoolDownDuration);
		}
        public override void Activate(ArmLogic Logic, ArmData data, GameObject shieldObj, Vector3 fromScale){
			armLogic = Logic;
			shield = shieldObj;
			_shieldCollider = shieldObj.GetComponentInChildren<Collider>();
			_shieldController = shieldObj.GetComponentInChildren<ShieldController>();
			UpdateShieldScale(data); 
			UpdateShieldPosition(data);

			armLogic.OnEnergyChanged += UpdateShieldScale;
			armLogic.OnEnergyChanged += UpdateShieldPosition;
			armLogic.OnUpdateInput += DetectDeviceSpeed;
			_shieldController.OnCollision += OnShieldCollide;

			base.Activate(Logic, data, shieldObj, fromScale);
		}
        public override void Deactivate(GameObject obj){
			if(armLogic != null){
				armLogic.OnEnergyChanged -= UpdateShieldScale;
				armLogic.OnUpdateInput -= DetectDeviceSpeed;
			}

			if(_shieldController) _shieldController.OnCollision -= OnShieldCollide;

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
		private void UpdateShieldPosition(ArmData data)
		{
			shield.transform.localPosition = _DefaultOffset +
				new Vector3(0, 0, _OffsetRange.x);
		}
		private void DetectDeviceSpeed(DeviceInputDetected inputDetected){
			if (armLogic.CheckFullEnergy()) return;
			if (absorbCooldown != null)
			{
				//print(absorbCooldown.CanInvoke());
				if (!absorbCooldown.CanInvoke()) return;
			}

			var selectorSpeed = inputDetected.Selector.Speed;
			if(selectorSpeed > speedLimit){
				timer += Time.fixedDeltaTime;
				if(timer > speedDuring){
					if (absorbProcess != null) return;

					StartCoroutine(Absorb(inputDetected));

					timer = 0;
				}
			}
		}
		private void OnShieldCollide(InteractType interactType, Collider selfCollider)
		{
			if (!armLogic.CheckHasEnergy()) return;
			// Spend Energy            
			armLogic.SpentEnergy(_SpentEnergyWhenCollide);
			// Shorten Sword
			UpdateShieldScale(armLogic.data);
		}
		private IEnumerator Absorb(DeviceInputDetected inputDetected) {
			absorbCooldown.Reset();

			// ReSharper disable once Unity.PreferNonAllocApi
			var raycastHits = Physics.SphereCastAll(shield.transform.position, range, shield.transform.forward);
			foreach (var hit in raycastHits)
			{
				var interactablePower = hit.transform.GetComponent<InteractablePower>();
				interactablePower?.OnSelect(inputDetected);
				yield return new WaitForSeconds(_AbsorbInterpolateDuration);
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