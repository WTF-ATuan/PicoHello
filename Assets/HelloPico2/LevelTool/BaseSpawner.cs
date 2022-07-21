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
        LineRenderer lineRenderer;
        TrajectoryMover trajectoryMover;

        private void OnValidate()
        {
            Notify?.Invoke(this);
        }
        [Button]
        private void AddTrajectoryModule()
        {
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = .3f;
                lineRenderer.material = Resources.Load("MAT_TrajectoryCurve", typeof(Material)) as Material;
                lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lineRenderer.receiveShadows = false;
            }
            if (trajectoryMover == null)
            {
                trajectoryMover = gameObject.AddComponent<TrajectoryMover>();
                trajectoryMover._Spawner = this;
                trajectoryMover._LineRenderer = lineRenderer;
                trajectoryMover.dist = 100;
            }
        }
        [Button]
        private void RemoveTrajectoryModule() {
            if (lineRenderer == null)
            {
                DestroyImmediate(lineRenderer);
            }
            if (trajectoryMover == null)
            {
                DestroyImmediate(trajectoryMover);
            }
        }
    }
}
