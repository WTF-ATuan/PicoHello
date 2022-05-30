using System.Collections;
using UnityEngine;
using HelloPico2.InteractableObjects;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.PlayerController.Arm
{
    [RequireComponent(typeof(ArmLogic))]
    public class SwordBehavior : WeaponBehavior
    {
        [SerializeField] private float _unitDuration = 0.01f;
        [SerializeField] private float _turnOffUnitDuration = 0.001f;
        [SerializeField] private float _speedLimit;
        [SerializeField] private float _returnDuring;
        private LightBeamRigController lightBeamRigController;

        private float timer;
        private ArmLogic armLogic { get; set; }        
        Coroutine stretchProcess;
        public override void Activate(ArmLogic Logic, ArmData data, GameObject lightBeam)
        {
            armLogic = Logic;
            
            if(lightBeamRigController == null) 
                lightBeamRigController = lightBeam.GetComponent<LightBeamRigController>();            

            UpdateSwordLength(data, _unitDuration);

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

            if (stretchProcess != null) StopCoroutine(stretchProcess);
            stretchProcess = StartCoroutine(TurnOffSwordSequence(obj));

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
            if (obj.Selector.Speed > _speedLimit)
            {
                lightBeamRigController.ModifyBlendWeight(0.01f);
                timer = 0;
            }
            else
            {
                timer += Time.fixedDeltaTime;
                if (timer > _returnDuring)
                {
                    lightBeamRigController.ModifyBlendWeight(-0.01f);
                }
            }
        }
        private void UpdateSwordLength(ArmData data) => UpdateSwordLength(data, 0);        
        private void UpdateSwordLength(ArmData data, float unitDuration = 0) {
            float energyPercentage = data.Energy / data.MaxEnergy;
            float dir = (energyPercentage > lightBeamRigController.GetUpdateState().TotalLength) ? 0.1f : -0.1f;
            
            //print(energyPercentage +"?: "+ lightBeamRigController.GetUpdateState().TotalLength + " : \n" + dir);
                        
            lightBeamRigController.SetLengthLimit(data.Energy / data.MaxEnergy);

            if(stretchProcess != null) StopCoroutine(stretchProcess); 
            stretchProcess = StartCoroutine(StretchSword(dir, unitDuration));

        }
        private IEnumerator StretchSword(float dir, float unitDuration) {
            while (Mathf.Abs(lightBeamRigController.GetUpdateState().TotalLength - GetLengthLimitPercentage()) > 0.1f)
            {
                lightBeamRigController.ModifyControlRigLength(dir);

                yield return new WaitForSeconds(unitDuration);
            }

            if (GetLengthLimitPercentage() == 0)
            {
                while (lightBeamRigController.GetUpdateState().TotalLength != 0)
                {
                    lightBeamRigController.ModifyControlRigLength(-0.1f);

                    yield return new WaitForSeconds(unitDuration);
                }
            }

            lightBeamRigController.SetRigTotalLength(GetlightBeamData().MaxLengthLimit * GetLengthLimitPercentage());
        }
        private IEnumerator TurnOffSwordSequence(GameObject obj) { 
            yield return StartCoroutine(TurnOffSword(_turnOffUnitDuration));
            obj.SetActive(false);
        }
        private IEnumerator TurnOffSword(float unitDuration)
        {
            while (lightBeamRigController.GetUpdateState().TotalLength != 0)
            {
                lightBeamRigController.ModifyControlRigLength(-0.1f);

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
