using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.InteractableObjects
{
    public class ObjectScaleRandomizer : MonoBehaviour
    {
        public Transform _ControlTarget;
        [MinMaxSlider(0.01f,10,true)] public Vector2 _ScaleRange = Vector2.one;
        public void SetRandomScale() { 
             _ControlTarget.transform.localScale = Vector3.one * Random.Range(_ScaleRange.x, _ScaleRange.y);
        }
        public void SetRandomScaleRatoi()
        {
            _ControlTarget.transform.localScale *= Random.Range(_ScaleRange.x, _ScaleRange.y);
        }
    }
}
