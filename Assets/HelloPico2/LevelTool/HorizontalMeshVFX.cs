using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.LevelTool
{
    public class HorizontalMeshVFX : MonoBehaviour
    {
        public Vector2 _AngleXRange = new Vector2(-5,5);
        [Button]
        public void SetUp() {
            var angle = Random.Range(-5, 5);
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
