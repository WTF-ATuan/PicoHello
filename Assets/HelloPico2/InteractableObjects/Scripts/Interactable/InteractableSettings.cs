using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.InteractableObjects
{
    [CreateAssetMenu(menuName = "HelloPico2/ScriptableObject/InteractableSettings")]
    public class InteractableSettings : ScriptableObject
    {
        public enum InteractableType { Sword, Shield, Whip}
        [System.Serializable]
        public struct InteractableData { 
            public InteractableType type;
            public GameObject prefab;
            public float energyCost;
        }
        public List<InteractableData> _InteractableDatas = new List<InteractableData>();

        public InteractableData GetInteractableObject(InteractableType type) {
            for (int i = 0; i < _InteractableDatas.Count; i++)
            {
                if (_InteractableDatas[i].type == type)
                    return _InteractableDatas[i];
            }

            Debug.Log("Couldn't find this prefab in interactableSettings: " + type.ToString());
            return _InteractableDatas[0];
        }
    }
}
