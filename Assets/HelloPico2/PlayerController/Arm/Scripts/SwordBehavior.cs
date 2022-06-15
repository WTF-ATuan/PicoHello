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
        [Header("Stretching Setttings")]
        /// <summary>
        /// The unit length for ModifyControllingRigLength func
        /// </summary>
        [SerializeField] private float _ModifyLengthStep = 0.3f;
        [SerializeField] private float _TurnOnDuration = 0.01f;
        [SerializeField] private float _TurnOffDuration = 0.01f;

        [Header("Blend Weight Color Settings")]
        [SerializeField] private string _ColorName = "_BaseColor";
        [SerializeField][ColorUsage(true,true)] private Color _ColorSword;
        [SerializeField][ColorUsage(true, true)] private Color _ColorWhip;

        [Header("Sword & Whip Transform Settings")]
        [SerializeField] private float _SpeedLimit;
        [SerializeField] private float _ReturnDuring;
        private LightBeamRigController lightBeamRigController;

        [Header("Collider Setting")]
        [SerializeField] private int _SpentEnergyWhenCollide = 30;

        private float timer;
        private SkinnedMeshRenderer _beamMesh;
        [MaxValue(1)][MinValue(0)] private float _colorValue;

        private ArmLogic armLogic { get; set; }        
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

            armLogic.OnEnergyChanged += UpdateSwordLength;
            armLogic.OnUpdateInput += SetBlendWeight;
            
            base.Activate(Logic, data, lightBeam, fromScale);
        }
        public override void Deactivate(GameObject obj)
        {
            if (armLogic != null)
            {
                armLogic.OnEnergyChanged -= UpdateSwordLength;
                armLogic.OnUpdateInput -= SetBlendWeight;
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

            print("Spend energy");
        }
        #region LightBeamController
        private void SetBlendWeight(DeviceInputDetected obj)
        {
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
