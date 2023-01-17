using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace HelloPico2.InteractableObjects{
	public class LightBeamRigController : MonoBehaviour{
		[SerializeField] [Required] private Transform rigRoot;

		[SerializeField] [ProgressBar(1, 25)] [MaxValue(50)]
		private int controlRigCount = 5;

		[FormerlySerializedAs("maxMinLenghtLimit")] [SerializeField] [MinMaxSlider(0, 20)]
		public Vector2Int minMaxLenghtLimit;

		[SerializeField] [ReadOnly] private float currentLengthLimit = 5;

		[SerializeField] [ProgressBar(0.1, 1)] [MaxValue(1)] [MinValue(0.1)]
		private float thickness = 1;

		private List<Transform> _rigs;

		private DynamicBone _dynamicBone;

		private CapsuleCollider _capsuleCollider;

		[SerializeField] private AnimationCurve lengthChangedCurve;

		[FoldoutGroup("RaycastCollider")] [SerializeField]
		private bool _useRaycastCollider = false;

		[FoldoutGroup("RaycastCollider")] [Range(1, 8)] [SerializeField]
		private int _percision = 10;

		[FoldoutGroup("RaycastCollider")] [SerializeField]
		private LayerMask _layer;

		[FoldoutGroup("RaycastCollider")] [SerializeField]
		private float _checkRaycastCDDuration = 0.1f;

		[FoldoutGroup("RaycastCollider")] [SerializeField]
		private float _radius = 0.15f;

		public List<IInteractCollide> colliders = new List<IInteractCollide>();
		public LightBeamLengthUpdated currentLengthUpdated{ get; private set; }
		private Game.Project.ColdDownTimer checkRaycastCDTimer{ get; set; }

		public delegate void OnCollisionDel(InteractType interactType, Collider other);

		public OnCollisionDel OnCollision;

		public void SetLengthLimit(float percentage){
			if(percentage > 1){
				throw new Exception($"inputValue is greater than 1 {percentage}");
			}

			var lerpValue = Mathf.Lerp(minMaxLenghtLimit.x, minMaxLenghtLimit.y, percentage);
			currentLengthLimit = lerpValue;
		}

		public void Floating(bool enable){
			if(enable){
				var rigTransform = transform;
				var targetPosition = rigTransform.position + rigTransform.right * 0.2f + rigTransform.up * 0.2f;
				rigTransform.DOMove(targetPosition, 1f).SetLoops(-1, LoopType.Yoyo);
			}
			else{
				transform.DOKill(true);
			}
		}

		[Button]
		public void ModifyControlRigLength(float rigLength){
			var totalAddOffset = rigRoot.forward * rigLength;
			if(IsLengthLessThanZero(totalAddOffset)){
				SetRigTotalLength(0);
				return;
			}

			if(IsLengthGreaterThanLimit(totalAddOffset) && rigLength > 0) return;
			var rigOffset = totalAddOffset / controlRigCount;
			var rigLocalOffset = rigRoot.InverseTransformVector(rigOffset);
			SetRigLength(controlRigCount, rigLocalOffset, true);
		}

		public void SetPositionLengthByPercent(float rigLength, float value){
			PostLengthUpdatedEvent(0);
			var targetPosition = rigRoot.forward * rigLength;
			var lerpPosition = Vector3.Lerp(Vector3.zero, targetPosition, value);
			var rigOffset = lerpPosition / controlRigCount;
			for(var i = 0; i < controlRigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				var addOffset = rigTransform.InverseTransformVector(rigOffset);
				rigTransform.localPosition = addOffset;
			}

			PostLengthUpdatedEvent(2);
		}

		[Button]
		public void SetRigTotalLength(float offsetMultiplier){
			var totalOffset = rigRoot.forward * offsetMultiplier;
			var rigCount = controlRigCount;
			var rigOffset = totalOffset / rigCount;
			var rigLocalOffset = rigRoot.InverseTransformVector(rigOffset);
			SetRigLength(rigCount, rigLocalOffset);
		}

		public LightBeamLengthUpdated GetUpdateState(){
			var lengthUpdated = new LightBeamLengthUpdated();
			var result = Vector3.zero;
			for(var index = 0; index < _rigs.Count; index++){
				try{
					var rig = _rigs[index];
					result += rig.localPosition;
				}
				catch(ArgumentNullException exception){
					Debug.LogWarning("catch Exception of argument null");
					break;
				}
			}

			var totalOffset = result * 10f;
			var singleOffset = _rigs.First().localPosition * 10f;
			lengthUpdated.TotalLength = totalOffset.magnitude;
			lengthUpdated.SingleLength = singleOffset.magnitude;
			lengthUpdated.TotalOffset = totalOffset;
			lengthUpdated.UpdateState = 0;
			lengthUpdated.CurrentLengthLimit = currentLengthLimit;
			lengthUpdated.MaxLengthLimit = minMaxLenghtLimit.y;
			lengthUpdated.MinLengthLimit = minMaxLenghtLimit.x;
			lengthUpdated.BlendWeight = _dynamicBone.m_BlendWeight;
			return lengthUpdated;
		}

		public float GetLengthModifiedCurveSpeed(){
			var lightBeamLengthUpdated = GetUpdateState();
			var totalLength = lightBeamLengthUpdated.TotalLength;
			var lengthLimit = lightBeamLengthUpdated.CurrentLengthLimit;
			var progress = totalLength / lengthLimit;
			var curveValue = lengthChangedCurve.Evaluate(progress);
			return curveValue;
		}

		public void Init(){
			_dynamicBone = GetComponent<DynamicBone>();
			_capsuleCollider = GetComponent<CapsuleCollider>();
			_rigs = rigRoot.GetComponentsInChildren<Transform>().ToList();
			_rigs.RemoveAt(0);
			checkRaycastCDTimer = new Game.Project.ColdDownTimer(_checkRaycastCDDuration);
		}

		private void ModifyThickness(float percent){
			var localScale = rigRoot.localScale;
			thickness = percent;
			//To Sword
			if(_dynamicBone.m_BlendWeight < 0.2){
				localScale.x = Mathf.Lerp(2.5f, 5f, thickness);
				localScale.y = Mathf.Lerp(10, 20, thickness);
				transform.localScale = new Vector3(1, 1, 0.6f);
				rigRoot.localScale = localScale;
				_capsuleCollider.radius = 0.08f;
			}
			//To whip
			else{
				localScale.x = Mathf.Lerp(5, 10, thickness);
				localScale.y = Mathf.Lerp(5, 10, thickness);
				transform.localScale = Vector3.one;
				_capsuleCollider.radius = 0.04f;
				rigRoot.localScale = localScale;
			}
		}

		private void Start(){
			Init();
		}

		private void Update(){
			if(_useRaycastCollider){
				if(!checkRaycastCDTimer.CanInvoke()) return;

				RaycastColliderEnter();
				checkRaycastCDTimer.Reset();
			}
		}

		/// <summary>
		///     OverwriteCurrentUpdatedState
		///     and control dynamicBone by updateState
		/// </summary>
		/// <param name="updateState">
		///     0 -> not start yet
		///     1 -> updating
		///     2 -> finish
		/// </param>
		private void PostLengthUpdatedEvent(int updateState){
			var lengthUpdated = GetUpdateState();
			lengthUpdated.UpdateState = updateState;
			if(updateState == 0) SetDynamicBoneActive(false);

			if(updateState == 2){
				UpdateBoneCollider(lengthUpdated);
				UpdateBeamThickness(lengthUpdated);
				SetDynamicBoneActive(true);
			}

			currentLengthUpdated = lengthUpdated;
		}

		[Button]
		public void ResetBeam(){
			PostLengthUpdatedEvent(0);
			PostLengthUpdatedEvent(2);
		}

		private void SetDynamicBoneActive(bool enable){
			if(!enable){
				_dynamicBone.m_Root = null;
				_dynamicBone.UpdateRoot();
			}
			else{
				_dynamicBone.m_Root = rigRoot;
				_dynamicBone.UpdateRoot();
				_dynamicBone.UpdateParameters();
			}
		}

		private void UpdateBeamThickness(LightBeamLengthUpdated lengthUpdated){
			var totalOffset = lengthUpdated.TotalOffset;
			var totalLength = rigRoot.TransformVector(totalOffset).magnitude;
			var finalPercent = 1 - totalLength * 0.1f + 0.1f;
			ModifyThickness(finalPercent);
		}

		private void UpdateBoneCollider(LightBeamLengthUpdated lengthUpdated){
			var totalOffset = lengthUpdated.TotalOffset;
			var totalLength = lengthUpdated.TotalLength;
			var centerOfCollider = totalOffset / 2;
			_capsuleCollider.center = centerOfCollider;
			_capsuleCollider.height = totalLength;
		}

		private void OnTriggerEnter(Collider other){
			var collides = other.gameObject.GetComponents<IInteractCollide>();
			collides.ForEach(c => c?.OnCollide(InteractType.Beam, _capsuleCollider));

			foreach(var item in collides)
				if(item != null){
					print("Collide " + other.name);
					OnCollision?.Invoke(InteractType.Beam, _capsuleCollider);
				}
		}

		private void RaycastColliderEnter(){
			colliders.Clear();

			Vector3 POS = _rigs[0].position;
			for(int i = 0; i < _rigs.Count - 2; i++){
				if(i != 0 && i % _percision == 0){
					var result = SingleSphereRaycast(POS, _rigs[i].position);
					if(result.hitAmount != 0) AddHitInfo(colliders, result.hitInfos);
					POS = _rigs[i].position;
				}
			}

			colliders.ForEach(c => c?.OnCollide(InteractType.Beam, _capsuleCollider));

			foreach(var item in colliders)
				if(item != null){
					//print("Hit " + item);
					OnCollision?.Invoke(InteractType.Beam, _capsuleCollider);
				}
		}

		private (int hitAmount, RaycastHit[] hitInfos) SingleRaycast(Vector3 from, Vector3 to){
			Ray ray = new Ray(from, (to - from));
			RaycastHit[] hitInfos = new RaycastHit[3];
			int hitAmount = Physics.RaycastNonAlloc(ray, hitInfos, Vector3.Distance(from, to), _layer);

			return (hitAmount, hitInfos);
		}

		private (int hitAmount, RaycastHit[] hitInfos) SingleSphereRaycast(Vector3 from, Vector3 to){
			Ray ray = new Ray(from, (to - from));
			RaycastHit[] hitInfos = new RaycastHit[3];
			int hitAmount = Physics.SphereCastNonAlloc(ray, _radius, hitInfos, Vector3.Distance(from, to), _layer);

			return (hitAmount, hitInfos);
		}

		private void AddHitInfo(List<IInteractCollide> cols, RaycastHit[] hitInfos){
			for(int i = 0; i < hitInfos.Length; i++){
				if(hitInfos[i].collider == null) continue;

				var interactables = hitInfos[i].collider.GetComponents<IInteractCollide>();
				if(interactables.Length != 0){
					// Remove repeat items
					for(int j = 0; j < interactables.Length; j++){
						if(!cols.Contains(interactables[j]))
							cols.Add(interactables[j]);
					}
				}
			}
		}

		private bool IsLengthLessThanZero(Vector3 addOffset){
			var lengthUpdated = GetUpdateState();
			var localAddOffset = rigRoot.InverseTransformVector(addOffset);
			var totalOffset = lengthUpdated.TotalOffset;
			return (totalOffset + localAddOffset).z < 0;
		}

		private bool IsLengthGreaterThanLimit(Vector3 addOffset){
			var lengthUpdated = GetUpdateState();
			var localAddOffset = rigRoot.InverseTransformVector(addOffset);
			var totalOffset = lengthUpdated.TotalOffset;
			var limitOffset = rigRoot.forward * currentLengthLimit;
			var localLimitOffset = rigRoot.InverseTransformVector(limitOffset);
			return (totalOffset + localAddOffset).z > localLimitOffset.z;
		}

		private void SetRigLength(int rigCount, Vector3 rigOffset, bool isAdd = false){
			PostLengthUpdatedEvent(0);
			for(var i = 0; i < rigCount; i++){
				var rig = _rigs[i];
				var rigTransform = rig.transform;
				if(isAdd){
					rigTransform.localPosition += rigOffset;
				}
				else{
					rigTransform.localPosition = rigOffset;
				}

				PostLengthUpdatedEvent(1);
			}

			PostLengthUpdatedEvent(2);
		}

		[Button]
		public void ModifyBlendWeight(float amount){
			var currentBlendWeight = _dynamicBone.m_BlendWeight;
			var nextBlendWeight = Mathf.Clamp01(currentBlendWeight + amount);
			_dynamicBone.m_BlendWeight = nextBlendWeight;
			_dynamicBone.UpdateRoot();
			_dynamicBone.UpdateParameters();
			ModifyThickness(thickness);
		}

		private void OnDrawGizmos(){
			Gizmos.color = Color.red;
			Vector3 POS = _rigs[0].position;

			for(int i = 0; i < _rigs.Count - 2; i++){
				if(i == 0) Gizmos.DrawWireSphere(POS, _radius);

				if(i != 0 && i % _percision == 0){
					Gizmos.DrawLine(POS, _rigs[i].position);
					Gizmos.DrawWireSphere(_rigs[i].position, _radius);
					POS = _rigs[i].position;
				}
			}
		}
	}
}