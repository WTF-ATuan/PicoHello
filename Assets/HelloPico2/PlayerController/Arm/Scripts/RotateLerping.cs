using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class RotateLerping : MonoBehaviour
    {
        [SerializeField] private Transform _Target;
        [SerializeField] private float _Length = 30f;

        private void Awake()
        {
        }
        private void Update()
        {                
            var dir = _Target.position - transform.position;

            transform.up = Vector3.Lerp(Vector3.up, dir, dir.magnitude / _Length); 
        }
    }
}
