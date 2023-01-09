using HelloPico2.InputDevice.Scripts;
using HelloPico2.Interface;
using Project;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class LightBeamSound : MonoBehaviour, IWeaponFeedbacks
    {
        [SerializeField] private HandType _HandType = HandType.Left;
        [System.Serializable]
        public struct WaveSFXData {
            public float SpeedLimit;
        }
        [SerializeField] private List<WaveSFXData> _WaveSFXDatas = new List<WaveSFXData>();
        [SerializeField] private float _SpeedLimit;
        [SerializeField] private float _SFXCDDuration = 1f;
        [FoldoutGroup("SFX Settings")][SerializeField] private string _WhipClipName;
        [FoldoutGroup("SFX Settings")][SerializeField] private string _SwordClipName;

        private InteractableSettings.InteractableType currentInteractableType = InteractableSettings.InteractableType.Whip;
        private Game.Project.ColdDownTimer SFXCDTimer;
        private LightBeamRigController _rigController;
        private float selectorSpeed;

        private void Start()
        {
            _rigController = GetComponent<LightBeamRigController>();
            EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
            SFXCDTimer = new Game.Project.ColdDownTimer(_SFXCDDuration);
        }
        private void OnDeviceInputDetected(DeviceInputDetected obj)
        {
            if (!gameObject.activeSelf) return;
            if (obj.Selector.HandType != _HandType) return;
            
            selectorSpeed = obj.Selector.Speed;
            CheckSpeedEvent(selectorSpeed);
        }
        public void OnSwithWeapon(InteractableSettings.InteractableType interactableType)
        {
            currentInteractableType = interactableType;
        }
        private void CheckSpeedEvent(float speedOfSelector)
        {
            for (int i = 0; i < _WaveSFXDatas.Count - 1; i++)
            {
                if(speedOfSelector > _WaveSFXDatas[i].SpeedLimit && speedOfSelector <= _WaveSFXDatas[i-1].SpeedLimit)
                    CheckPlaySFX();
            }

            //if (speedOfSelector > _SpeedLimit)
            //{
            //    CheckPlaySFX();
            //}            
        }
        private void CheckPlaySFX() {
            if (!SFXCDTimer.CanInvoke()) return;

            if(currentInteractableType == InteractableSettings.InteractableType.Whip)
                AudioPlayerHelper.PlayAudio(_WhipClipName, transform.position);
            if(currentInteractableType == InteractableSettings.InteractableType.Sword)
                AudioPlayerHelper.PlayAudio(_SwordClipName, transform.position);

            SFXCDTimer.Reset();
        }
    }
}