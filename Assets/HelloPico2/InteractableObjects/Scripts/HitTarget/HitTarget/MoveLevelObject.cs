using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class MoveLevelObject : MonoBehaviour
    {
        public Vector3 dir { get; set; }
        public float speed { get; set; }
        public MoveLevelObject(Vector3 _dir, float _speed) { 
            dir = _dir;
            speed = _speed;
        }
        private void Update()
        {
            transform.Translate(dir * speed * Time.deltaTime, Space.World);
        }
    }
}
