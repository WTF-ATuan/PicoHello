using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class UnparentVFX : MonoBehaviour
    {
        [SerializeField] private float _DestroyDuration = 3f;
        public void Unparent()
        {
            transform.SetParent(transform.root.parent);
            Destroy(gameObject, _DestroyDuration);
        }
    }
}
