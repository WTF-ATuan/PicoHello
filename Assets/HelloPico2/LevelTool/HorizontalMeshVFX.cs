using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.LevelTool
{
    public class HorizontalMeshVFX : MonoBehaviour
    {
        public Vector2 _AngleXRange = new Vector2(-5,5);
        public Vector2 _MinAngleXRange = new Vector2(-2,2);
        [Button]
        public void SetUp() {

            float dir = Random.Range(-1, 1);
            float angle = 0f;

            if(dir > 0)
                angle = Random.Range(_MinAngleXRange.y, _AngleXRange.y);
            else
                angle = Random.Range(-_AngleXRange.x, -_MinAngleXRange.x);

            var pos = transform.parent.parent.position;
            transform.SetParent(transform.root.parent);
            transform.position = pos;
            transform.GetComponent<ParticleSystem>().Play(true);
            transform.up = Vector3.up;
            transform.forward = (Camera.main.transform.position - transform.position).normalized;
            transform.Rotate(transform.right, angle, Space.World);
            Destroy(gameObject, 3);
        }
    }
}
