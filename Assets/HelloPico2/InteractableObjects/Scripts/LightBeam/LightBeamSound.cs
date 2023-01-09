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
            public string WhipWaveClipName;
            public string SwordWaveClipName;
        }
        [SerializeField] private List<WaveSFXData> _WaveSFXDatas = new List<WaveSFXData>();
        [SerializeField] private float _SFXCDDuration = 1f;

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
                if(speedOfSelector > _WaveSFXDatas[i].SpeedLimit && speedOfSelector <= _WaveSFXDatas[i+1].SpeedLimit)
                    CheckPlaySFX(_WaveSFXDatas[i]);
            }       
        }
        private void CheckPlaySFX(WaveSFXData data) {
            if (!SFXCDTimer.CanInvoke()) return;

            if(currentInteractableType == InteractableSettings.InteractableType.Whip)
                AudioPlayerHelper.PlayMultipleAudio(data.WhipWaveClipName, transform.position);
            if(currentInteractableType == InteractableSettings.InteractableType.Sword)
                AudioPlayerHelper.PlayMultipleAudio(data.SwordWaveClipName, transform.position);

            SFXCDTimer.Reset();
        }
    }
}