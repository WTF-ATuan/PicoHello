using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.InteractableObjects;

namespace HelloPico2.PlayerController.Arm
{
    [RequireComponent(typeof(ArmLogic))]
    public class SwordBehavior : MonoBehaviour
    {
        [SerializeField] private Vector2 _LengthRange;
        LightBeamRigController lightBeamRigController;
        ArmLogic armLogic { get; set; }

        public void Activate(ArmLogic Logic, ArmData data, LightBeamRigController lightBeam)
        {
            armLogic = Logic;
            lightBeamRigController = lightBeam.GetComponent<LightBeamRigController>();
            UpdateSwordLength(data);
            armLogic.OnEnergyChanged += UpdateSwordLength;
        }
        public void Deactivate()
        {
            armLogic.OnEnergyChanged -= UpdateSwordLength;
        }

        private void UpdateSwordLength(ArmData data) {
            var targetLength = Mathf.Lerp(_LengthRange.x, _LengthRange.y, data.Energy / data.MaxEnergy);
            if (data.Energy == 0) targetLength = 0;
            //lightBeamRigController.ResetRigPosition();
            lightBeamRigController.ModifyControlRigLenght(targetLength);
        }
    }
}
