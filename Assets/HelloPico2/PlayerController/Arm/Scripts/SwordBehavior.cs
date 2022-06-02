using System.Collections;
using UnityEngine;
using HelloPico2.InteractableObjects;
using HelloPico2.InputDevice.Scripts;

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
        [Header("Sword & Whip Transform Settings")]
        [SerializeField] private float _SpeedLimit;
        [SerializeField] private float _ReturnDuring;
        private LightBeamRigController lightBeamRigController;

        private float timer;
        private ArmLogic armLogic { get; set; }        
        Coroutine TurnOffProcess;
        Coroutine stretchProcess;
        public override void Activate(ArmLogic Logic, ArmData data, GameObject lightBeam)
        {
            armLogic = Logic;

            // Initiate LightBeam
            if (lightBeamRigController == null)
            { 
                lightBeamRigController = lightBeam.GetComponent<LightBeamRigController>(); 
                lightBeamRigController.Init();
            }

            UpdateSwordLength(data, _TurnOnDuration);

            armLogic.OnEnergyChanged += UpdateSwordLength;
            armLogic.OnUpdateInput += SetBlendWeight;

            base.Activate(Logic, data, lightBeam);
        }
        public override void Deactivate(GameObject obj)
        {
            if (armLogic != null)
            {
                armLogic.OnEnergyChanged -= UpdateSwordLength;
                armLogic.OnUpdateInput -= SetBlendWeight;
            }

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
        #region LightBeamController
        private void SetBlendWeight(DeviceInputDetected obj)
        {
            if (obj.Selector.Speed > _SpeedLimit)
            {
                lightBeamRigController.ModifyBlendWeight(0.01f);
                timer = 0;
            }
            else
            {
                timer += Time.fixedDeltaTime;
                if (timer > _ReturnDuring)
                {
                    lightBeamRigController.ModifyBlendWeight(-0.01f);
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
            var unitDuration = duration * _ModifyLengthStep / (GetlightBeamData().MaxLengthLimit * GetLengthLimitPercentage());

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
