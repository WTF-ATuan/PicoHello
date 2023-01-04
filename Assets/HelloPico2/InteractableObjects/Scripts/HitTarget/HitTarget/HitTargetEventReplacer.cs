using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    public class HitTargetEventReplacer : MonoBehaviour
    {
        [SerializeField] private HitTargetRock _Rock;
        [SerializeField] private bool _UseChangeLayer = false;
        [ShowIf("_UseChangeLayer")][SerializeField] private string _ChangeToLayerName;
        public UltEvent WhenCollideUlt;

        public void ReplaceEvent() { 
            if(_Rock == null || !_UseChangeLayer) return;
            _Rock.WhenCollideUlt = WhenCollideUlt;
            gameObject.layer = LayerMask.NameToLayer(_ChangeToLayerName);
        }

    }
}
