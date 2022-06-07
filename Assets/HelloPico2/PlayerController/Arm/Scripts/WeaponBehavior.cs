using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm
{
    public abstract class WeaponBehavior : MonoBehaviour
    {
        public float _ActiveDuration;
        public float _DeactiveDuration;
        public delegate void FinishedDeactivate();
        public FinishedDeactivate _FinishedDeactivate;
        public virtual void Activate(ArmLogic Logic, ArmData data, GameObject Obj, Vector3 fromScale) { 
        
        }
        public virtual void Deactivate(GameObject obj) {

        }
    }
}
