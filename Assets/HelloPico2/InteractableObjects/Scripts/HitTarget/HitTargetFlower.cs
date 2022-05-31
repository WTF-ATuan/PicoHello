using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetFlower : HitTargetBase
    {
        public UnityEngine.Events.UnityEvent WhenCollide;
        public override void OnCollide()
        {
            base.OnCollide();
            WhenCollide?.Invoke();
        }
    }
}
