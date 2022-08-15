using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class FacingTarget : MonoBehaviour
    {
        public Transform _FacingThis;
        public Vector3 _FacingPoint;
        public float _FacingSpeed = 0.1f;
        private float step { get; set; }
        public FacingTarget(Transform target, float speed) { 
            _FacingThis = target;
            _FacingSpeed = speed;
        }
        public void UpdateToFacingPoint(Vector3 target) {
            _FacingThis = null;
            _FacingPoint = target;
            step = 0;
        }
        private void Update()
        {
            if (_FacingThis == null && _FacingPoint == Vector3.zero) { step = 0; return; }
            
            step += Time.fixedDeltaTime * _FacingSpeed;

            if (_FacingThis)
            {
                Facing(_FacingThis.position);
            }
            if (_FacingPoint != Vector3.zero) { 
                Facing(_FacingPoint);                
            }
        }
        private void Facing(Vector3 pos) {
            var dir = (pos - transform.position).normalized;
            var forward = Vector3.Lerp(transform.forward, dir, step);
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}
