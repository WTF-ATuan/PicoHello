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
    }
}
