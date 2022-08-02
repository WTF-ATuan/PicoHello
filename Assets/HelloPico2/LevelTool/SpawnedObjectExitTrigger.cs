using UnityEngine;
using HelloPico2.InteractableObjects;

namespace HelloPico2.LevelTool
{
    public class SpawnedObjectExitTrigger : TriggerBase
    {
        private void OnEnable()
        {
            TriggerEnter += TargetEnter;
        }
        private void OnDisable()
        {
            TriggerEnter -= TargetEnter;            
        }
        private void TargetEnter(Collider other)
        {
            if (other.transform.parent.TryGetComponent<ObjectScaler>(out var scaler)) {
                scaler.StartScaling(0);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1,0,0,.3f);
            Gizmos.DrawCube(transform.position, transform.localScale);
        }
    }
}
