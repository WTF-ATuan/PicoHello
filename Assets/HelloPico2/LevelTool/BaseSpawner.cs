using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.LevelTool
{
    public abstract class BaseSpawner : MonoBehaviour
    {
        [EnumPaging] public enum SpawnType { Interactable, HitTarget}
        public SpawnType _SpawnType;
        public HelloPico2.LevelTool.SpawnersManager.SpawnDirection _SpawnDirection;
        [ShowIf("_SpawnType", SpawnType.Interactable)]public string _InteractableType;
        [ShowIf("_SpawnType", SpawnType.HitTarget)] public string _HitTargetType;
        public float _Speed;   
        public bool _UseExternalForce = false;   
        [ShowIf("_UseExternalForce")] public Vector3 _ForceDir = Vector3.zero;
        [ShowIf("_UseExternalForce")] public float _Force = 0; 
        public System.Action<BaseSpawner> Notify;
        private void OnValidate()
        {
            Notify?.Invoke(this);
        }
    }
}
