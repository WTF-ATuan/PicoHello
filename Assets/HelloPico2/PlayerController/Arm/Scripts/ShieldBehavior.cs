using UnityEngine;
using HelloPico2.InteractableObjects;
using DG.Tweening;

namespace HelloPico2.PlayerController.Arm
{
    [RequireComponent(typeof(ArmLogic))]
    public class ShieldBehavior : MonoBehaviour
    {
        [SerializeField] private Vector2 _ScaleRange;
        [SerializeField] private float _ScalingDuration;
        //LightBeamRigController lightBeamRigController;
        GameObject shield { get; set; }
        ArmLogic armLogic { get; set; }
        public void Activate(ArmLogic Logic, ArmData data, GameObject shieldObj)
        {
            armLogic = Logic;
            //lightBeamRigController = lightBeam.GetComponent<LightBeamRigController>();
            shield = shieldObj;
            UpdateShieldScale(data);
            armLogic.OnEnergyChanged += UpdateShieldScale;
        }
        public void Deactivate()
        {
            if (armLogic != null) 
                armLogic.OnEnergyChanged -= UpdateShieldScale;
        }
        private void OnDisable()
        {
            Deactivate();
        }
        private void UpdateShieldScale(ArmData data)
        {
            var targetScale = Mathf.Lerp(_ScaleRange.x, _ScaleRange.y, data.Energy / data.MaxEnergy);
            if (data.Energy == 0) targetScale = 0;
            shield.transform.DOScale(new Vector3(targetScale, targetScale, shield.transform.localScale.z) , _ScalingDuration);
        }
    }
}
