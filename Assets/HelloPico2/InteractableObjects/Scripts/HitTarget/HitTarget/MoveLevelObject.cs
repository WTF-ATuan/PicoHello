using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class MoveLevelObject : MonoBehaviour
    {
        public Vector3 dir { get; set; }
        public float speed { get; set; }
        public bool useGravity = false;
        public float gravity = 0;
        private float time { get; set; }
        public MoveLevelObject(Vector3 _dir, float _speed, bool _useGravity = false, float _gravity = -9.8f) { 
            dir = _dir;
            speed = _speed;

            if (_useGravity) { 
                useGravity = _useGravity;
                gravity = _gravity;
            }
        }
        private void Update()
        {
            transform.Translate(dir * speed * Time.deltaTime, Space.World);

            if (useGravity)
            {
                time += Time.deltaTime;
                var speed1 = gravity * Mathf.Pow(time, 2);
                transform.Translate(transform.up * speed1 * Time.deltaTime, Space.World);
            }
        }
    }
}
