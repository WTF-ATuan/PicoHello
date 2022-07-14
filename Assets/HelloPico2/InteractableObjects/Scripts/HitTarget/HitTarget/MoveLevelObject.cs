using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class MoveLevelObject : MonoBehaviour, ITrackInteractableState
    {
        public Vector3 dir { get; set; }
        public float speed { get; set; }
        public bool useExternalForce = false;
        public Vector3 forceDir = Vector3.up;
        public float force = 0;
        private float time { get; set; }
        public void SetUpMoveLevelObject(Vector3 _dir, float _speed, bool _useExternalForce = false, Vector3 _forceDir = default, float _Force = -9.8f) { 
            dir = _dir;
            speed = _speed;

            if (_useExternalForce) { 
                useExternalForce = _useExternalForce;
                forceDir = (_forceDir == default) ?  Vector3.up : _forceDir;
                force = _Force;
            }
        }
        private void Update()
        {
            if (!useExternalForce) 
            { 
                transform.Translate(dir * speed * Time.fixedDeltaTime, Space.World); 
            }
            else
            {
                time += Time.fixedDeltaTime;
                
                var Move = dir * speed * Time.fixedDeltaTime;

                Move += forceDir * force / 2 * Mathf.Pow(time, 1);

                transform.Translate(Move, Space.World);
            }
        }
        public void WhenCollideWith(HelloPico2.InteractableObjects.InteractType type)
        {
            speed = 0;
        }
    }
    public interface ITrackInteractableState {
        public void WhenCollideWith(HelloPico2.InteractableObjects.InteractType type);
    }
}
