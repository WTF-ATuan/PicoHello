using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class RotateLerping : MonoBehaviour
    {
        [SerializeField] private Transform _Target;
        [Range(0,1)][SerializeField] private float _Wavyness = 0;
        [SerializeField] private float _BufferDistance = 30f;

        private void Awake()
        {
        }
        private void Update()
        {
            var targetPos = _Target.position;
            targetPos.y -= _Wavyness * targetPos.y; 
            var dir = targetPos - transform.position;

            transform.up = Vector3.Lerp(Vector3.up, dir, dir.magnitude / _BufferDistance); 
        }
    }
}
