using HelloPico2.InputDevice.Scripts;
using HelloPico2.Interface;
using Project;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.XR.PXR.PXR_HandPoseConfig.BonesRecognizer;

namespace HelloPico2.InteractableObjects
{
    public class LightBeamSound : MonoBehaviour, IWeaponFeedbacks
    {
        [SerializeField] private HandType _HandType = HandType.Left;
        [System.Serializable]
        public struct WaveSFXData {
            public float PositionChangeAngle;
            public string WhipWaveClipName;
            public string SwordWaveClipName;
        }
        [SerializeField] private List<WaveSFXData> _WaveSFXDatas = new List<WaveSFXData>();
        [SerializeField] private float _SFXCDDuration = 1f;
        [SerializeField] private float _CheckPositionChangeAngleCycle = 0.1f;
        [SerializeField] private float _ActivateDistance = 0.3f;
        [ReadOnly][SerializeField] private float _PositionChangeAngle;
        [ReadOnly][SerializeField] private float _PositionChangeDistance;

        private InteractableSettings.InteractableType currentInteractableType = InteractableSettings.InteractableType.Whip;
        private Game.Project.ColdDownTimer SFXCDTimer;
        private Game.Project.ColdDownTimer CheckSpeedDifferenceTimer;
        private LightBeamRigController _rigController;
        private float selectorSpeed;

        private Vector3 Pos = Vector3.zero;
        private Vector3 Dir = Vector3.forward;
        private Vector3 CurrentDir;

        private void Start()
        {
            _rigController = GetComponent<LightBeamRigController>();
            EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
            SFXCDTimer = new Game.Project.ColdDownTimer(_SFXCDDuration);
            CheckSpeedDifferenceTimer = new Game.Project.ColdDownTimer(_CheckPositionChangeAngleCycle);
        }
        private void OnDeviceInputDetected(DeviceInputDetected obj)
        {
            if (!gameObject.activeSelf) return;
            if (obj.Selector.HandType != _HandType) return;
            
            selectorSpeed = obj.Selector.Speed;

            CheckDirection();
        }
        public void OnSwithWeapon(InteractableSettings.InteractableType interactableType)
        {
            currentInteractableType = interactableType;
        }        
        private void CheckDirection() {
            if (!CheckSpeedDifferenceTimer.CanInvoke()) return;

            _PositionChangeDistance = Vector3.Distance(transform.position, Pos);
            if(_PositionChangeDistance < _ActivateDistance) return;

            CurrentDir = transform.position - Pos;

            _PositionChangeAngle = Vector3.Angle(Dir, CurrentDir);
            
            for (int i = 0; i < _WaveSFXDatas.Count - 1; i++)
            {
                if (_PositionChangeAngle > _WaveSFXDatas[i].PositionChangeAngle && _PositionChangeAngle <= _WaveSFXDatas[i + 1].PositionChangeAngle)
                  CheckPlaySFX(_WaveSFXDatas[i]);
            } 
            if (_PositionChangeAngle > _WaveSFXDatas[_WaveSFXDatas.Count - 1].PositionChangeAngle)
            {
                CheckPlaySFX(_WaveSFXDatas[_WaveSFXDatas.Count - 1]);
            }
            
            Dir = CurrentDir;
            Pos = transform.position;
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