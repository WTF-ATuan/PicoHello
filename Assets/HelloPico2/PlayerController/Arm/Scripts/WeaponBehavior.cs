using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm
{
    public abstract class WeaponBehavior : MonoBehaviour
    {
        [FoldoutGroup("Common Settings")] public float _ActiveDuration;
        [FoldoutGroup("Common Settings")] public float _DeactiveDuration;
        [FoldoutGroup("Common Settings")][SerializeField] protected Vector3 _DefaultOffset = new Vector3(0, 0, 0);
        [FoldoutGroup("Common Settings")][SerializeField] protected Vector2 _OffsetRange;

        public delegate void FinishedDeactivate();
        public FinishedDeactivate _FinishedDeactivate;
        public virtual void Activate(ArmLogic Logic, ArmData data, GameObject Obj, Vector3 fromScale) { 
        
        }
        public virtual void Deactivate(GameObject obj) {

        }
    }
}
