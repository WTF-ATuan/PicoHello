using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.LevelTool
{
    public class SkinnedMeshEffectPlacement : MonoBehaviour
    {
        Transform _obj;
        Renderer _place;
        public UltEvents.UltEvent WhenSetPosition;

        [Button]
        public void SetPosition(Renderer place, Transform obj) {
            _place = place;
            _obj = obj;
            WhenSetPosition?.Invoke();
        }
        private void Update()
        {
            if(_place != null && _obj != null)
                _obj.position = _place.bounds.center;
        }
        private void OnDisable()
        {
            _place = null;
        }
    }
}
