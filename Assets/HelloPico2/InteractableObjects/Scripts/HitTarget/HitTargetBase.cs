using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetBase : MonoBehaviour, IBeamCollide
    {
        public virtual void OnCollide()
        {

        }
    }
}
