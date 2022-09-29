using System.Collections;
using UnityEngine;
using HelloPico2.InteractableObjects;
using HelloPico2.InputDevice.Scripts;
using Sirenix.OdinInspector;

namespace HelloPico2.PlayerController.Arm
{
    [RequireComponent(typeof(ArmLogic))]
    public class SwordBehavior : WeaponBehavior
    {
        /// <summary>
        /// The unit length for ModifyControllingRigLength func
        /// </summary>
        [FoldoutGroup("Stretching")][SerializeField] private float _ModifyLengthStep = 0.3f;
        [FoldoutGroup("Stretching")][SerializeField] private float _TurnOnDuration = 0.01f;
        [FoldoutGroup("Stretching")][SerializeField] private float _TurnOffDuration = 0.01f;
        [FoldoutGroup("Stretching")][SerializeField] private float _SwitchTypeSpeed = 0.01f;

        [FoldoutGroup("Interaction")][SerializeField] private int _SpentEnergyWhenCollide = 30;

        [FoldoutGroup("Blend Weight Color")][SerializeField] private string _ColorName = "_BaseColor";
        [FoldoutGroup("Blend Weight Color")][SerializeField][ColorUsage(true,true)] private Color _ColorSword;
        [FoldoutGroup("Blend Weight Color")][SerializeField][ColorUsage(true, true)] private Color _ColorWhip;

        [FoldoutGroup("Velocity Detection Settings")][SerializeField] private float _SpeedLimit;
        [FoldoutGroup("Velocity Detection Settings")][SerializeField] private float _ReturnDuring;

        public enum State { sword, whip}
        [FoldoutGroup("Debug")][ReadOnly][SerializeField] private State _State = State.whip;
        private LightBeamRigController lightBeamRigController;
        private float timer;
        private SkinnedMeshRenderer _beamMesh;
        [MaxValue(1)][MinValue(0)] private float _colorValue;

        public bool _TriggerControlBlendWeight = false;

        private ArmLogic armLogic { get; set; }
        InteractableSettings.InteractableType currentInteractableType;
        public UnityEngine.Events.UnityAction<InteractableSettings.InteractableType> WhenCollide;
        Coroutine TurnOffProcess;
        Coroutine stretchProcess;
        
        public override void Activate(ArmLogic Logic, ArmData data, GameObject lightBeam, Vector3 fromScale)
        {
            armLogic = Logic;

            // Initiate LightBeam
            if (lightBeamRigController == null)
            { 
                lightBeamRigController = lightBeam.GetComponentInChildren<LightBeamRigController>();
                _beamMesh = lightBeamRigController.GetComponentInChildren<SkinnedMeshRenderer>();
                lightBeamRigController.Init();
            }

            if (lightBeamRigController) lightBeamRigController.OnCollision += OnBeamCollide;

            UpdateSwordLength(data, _TurnOnDuration);
            UpdateSwordPosition(data);

            armLogic.OnEnergyChanged += UpdateSwordLength;
            armLogic.OnEnergyChanged += UpdateSwordPosition;
            
            if (!_TriggerControlBlendWeight)
                armLogic.OnUpdateInput += SetBlendWeight;
            else
            {
                armLogic.OnTriggerUp += ActivateWhip;
                armLogic.OnTriggerDown += ActivateSword;
            }

            base.Activate(Logic, data, lightBeam, fromScale);
        }

        public override void Deactivate(GameObject obj)
        {
            if (armLogic != null)
            {
                armLogic.OnEnergyChanged -= UpdateSwordLength;

                if (!_TriggerControlBlendWeight)
                    armLogic.OnUpdateInput -= SetBlendWeight;
                else
                { 
                    armLogic.OnTriggerUp -= ActivateWhip; 
                    armLogic.OnTriggerDown -= ActivateSword; 
                }
            }

            if(lightBeamRigController) lightBeamRigController.OnCollision -= OnBeamCollide;

            if (TurnOffProcess != null) { StopCoroutine(TurnOffProcess); }
            if (stretchProcess != null) { StopCoroutine(stretchProcess); }
            
            TurnOffProcess = StartCoroutine(TurnOffSwordSequence(obj));
            

            base.Deactivate(obj);
        }
        private void OnDisable()
        {
            //Deactivate(); 
            StopAllCoroutines();
        }
        private void OnBeamCollide(InteractType interactType, Collider selfCollider) {
            if (!armLogic.CheckHasEnergy()) return;
            // Spend Energy            
            armLogic.SpentEnergy(_SpentEnergyWhenCollide);
            // Shorten Sword
            UpdateSwordLength(armLogic.data);
            
            print(WhenCollide);
            WhenCollide?.Invoke(currentInteractableType);
        }
        #region LightBeamController
        private void ActivateSword(ArmData data) 
        {
            lightBeamRigController.ModifyBlendWeight(-_SwitchTypeSpeed);

            _colorValue -= _SwitchTypeSpeed;
            _colorValue = Mathf.Clamp(_colorValue, 0, 1);
            var lerpColor = Color.Lerp(_ColorSword, _ColorWhip, _colorValue);
            _beamMesh.material.SetColor(_ColorName, lerpColor);

            _State = State.sword;
            currentInteractableType = InteractableSettings.InteractableType.Sword;
        }
        private void ActivateWhip(ArmData data)
        {
            lightBeamRigController.ModifyBlendWeight(_SwitchTypeSpeed);

            _colorValue += _SwitchTypeSpeed;
            _colorValue = Mathf.Clamp(_colorValue, 0, 1);
            var lerpColor = Color.Lerp(_ColorSword, _ColorWhip, _colorValue);
            _beamMesh.material.SetColor(_ColorName, lerpColor);
            timer = 0;

            _State = State.whip;
            currentInteractableType = InteractableSettings.InteractableType.Whip;
        }
        private void SetBlendWeight(DeviceInputDetected obj)
        {
            if (obj.Selector.HandType != armLogic.data.HandType) return;

            if (obj.Selector.Speed > _SpeedLimit)
            {
                lightBeamRigController.ModifyBlendWeight(0.01f); 
                
                _colorValue += 0.01f;
                _colorValue = Mathf.Clamp(_colorValue, 0, 1);
                var lerpColor = Color.Lerp(_ColorSword, _ColorWhip, _colorValue);
                _beamMesh.material.SetColor(_ColorName, lerpColor);
                timer = 0;
            }
            else
            {
                timer += Time.fixedDeltaTime;
                if (timer > _ReturnDuring)
                {
                    lightBeamRigController.ModifyBlendWeight(-0.01f); 
                    
                    _colorValue -= 0.01f;
                    _colorValue = Mathf.Clamp(_colorValue, 0, 1);
                    var lerpColor = Color.Lerp(_ColorSword, _ColorWhip, _colorValue);
                    _beamMesh.material.SetColor(_ColorName ,lerpColor);
                }
            }
        }
        private void UpdateSwordLength(ArmData data) => UpdateSwordLength(data, 0);        
        private void UpdateSwordLength(ArmData data, float duration = 0) {
            float energyPercentage = data.Energy / data.MaxEnergy;
            float dir = (energyPercentage > lightBeamRigController.GetUpdateState().TotalLength) ? _ModifyLengthStep : -_ModifyLengthStep;
            
            //print(energyPercentage +"?: "+ lightBeamRigController.GetUpdateState().TotalLength + " : \n" + dir);
                        
            lightBeamRigController.SetLengthLimit(data.Energy / data.MaxEnergy);

            if(stretchProcess != null) StopCoroutine(stretchProcess);

            stretchProcess = StartCoroutine(StretchSword(dir, duration));
        }
        private void UpdateSwordPosition(ArmData data)
        {
            lightBeamRigController.transform.localPosition = _DefaultOffset +
            new Vector3(0, 0, _OffsetRange.x);
        }
        private IEnumerator StretchSword(float dir, float duration) {
            float totalLengthPercentage = GetlightBeamData().MaxLengthLimit * GetLengthLimitPercentage();
            float unitDuration = duration * _ModifyLengthStep / totalLengthPercentage;

            if ( totalLengthPercentage == 0) unitDuration = 0;

            while (Mathf.Abs(lightBeamRigController.GetUpdateState().TotalLength - GetLengthLimitPercentage()) > _ModifyLengthStep)
            {
                lightBeamRigController.ModifyControlRigLength(dir);

                yield return new WaitForSeconds(unitDuration);
            }

            if (GetLengthLimitPercentage() == 0)
            {
                while (lightBeamRigController.GetUpdateState().TotalLength != 0)
                {
                    lightBeamRigController.ModifyControlRigLength(-_ModifyLengthStep);

                    yield return new WaitForSeconds(unitDuration);
                }
            }

            lightBeamRigController.SetRigTotalLength(GetlightBeamData().MaxLengthLimit * GetLengthLimitPercentage());
        }
        private IEnumerator TurnOffSwordSequence(GameObject obj) {
            stretchProcess = StartCoroutine(TurnOffSword(_TurnOffDuration));
            yield return stretchProcess;
            obj.SetActive(false);
            _FinishedDeactivate?.Invoke();
        }
        private IEnumerator TurnOffSword(float duration)
        {
            var unitDuration = duration * _ModifyLengthStep / lightBeamRigController.GetUpdateState().TotalLength;

            while (lightBeamRigController.GetUpdateState().TotalLength != 0)
            {
                lightBeamRigController.ModifyControlRigLength(-_ModifyLengthStep);

                yield return new WaitForSeconds(unitDuration);
            }

            lightBeamRigController.SetRigTotalLength(0);
        }
        private LightBeamLengthUpdated GetlightBeamData()
        {
            return lightBeamRigController.GetUpdateState();
        }
        private float GetLengthLimitPercentage() {
            var lightBeamData = lightBeamRigController.GetUpdateState();
            return lightBeamData.CurrentLengthLimit / lightBeamData.MaxLengthLimit;
        }
        #endregion
    }
}
