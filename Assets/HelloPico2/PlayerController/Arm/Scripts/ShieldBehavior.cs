using UnityEngine;
using HelloPico2.InteractableObjects;
using DG.Tweening;

namespace HelloPico2.PlayerController.Arm
{
    [RequireComponent(typeof(ArmLogic))]
    public class ShieldBehavior : WeaponBehavior
    {
        [SerializeField] private Vector2 _ScaleRange;
        [SerializeField] private float _ScalingDuration;

        GameObject shield { get; set; }
        ArmLogic armLogic { get; set; }
        public override void Activate(ArmLogic Logic, ArmData data, GameObject shieldObj, Vector3 fromScale)
        {
            armLogic = Logic;

            shield = shieldObj;
            UpdateShieldScale(data);
            armLogic.OnEnergyChanged += UpdateShieldScale;

            base.Activate(Logic, data, shieldObj, fromScale);
        }
        public override void Deactivate(GameObject obj)
        {
            if (armLogic != null) 
                armLogic.OnEnergyChanged -= UpdateShieldScale;

            shield.transform.DOScale(new Vector3(0, 0, shield.transform.localScale.z), _DeactiveDuration).OnComplete(() => { 
                obj.SetActive(false);
                _FinishedDeactivate?.Invoke();
            });

            base.Deactivate(obj);
        }
        private void OnDisable()
        {
            //Deactivate();
            StopAllCoroutines();
        }
        private void UpdateShieldScale(ArmData data)
        {
            var targetScale = Mathf.Lerp(_ScaleRange.x, _ScaleRange.y, data.Energy / data.MaxEnergy);
            if (data.Energy == 0) targetScale = 0;
            shield.transform.DOScale(new Vector3(targetScale, targetScale, shield.transform.localScale.z) , _ScalingDuration);
        }
    }
}
