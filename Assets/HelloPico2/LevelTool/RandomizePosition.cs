using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class RandomizePosition : MonoBehaviour
    {
        public bool _RunOnEnable = true;
        public float _UpdatePositionFrq = 2f;

        public Vector3 _PositionRange = Vector3.zero;

        public Vector3 OriginalPosition = Vector3.zero;
        
        Coroutine process;
        private void OnEnable()
        {
            OriginalPosition = transform.position;
            
            if (_RunOnEnable)
                process = StartCoroutine(RandomPositioning());
        }
        private void OnDisable()
        {
            StopCoroutine(process);
            process = null;
        }
        private IEnumerator RandomPositioning() {            
            while (true)
            {
                transform.position = OriginalPosition + GetRandomPosition();
                yield return new WaitForSeconds(1/ _UpdatePositionFrq);
            }
        }
        private Vector3 GetRandomPosition()
        {
            return new Vector3 (
                    Random.Range(-_PositionRange.x, _PositionRange.x), 
                    Random.Range(-_PositionRange.y, _PositionRange.y), 
                    Random.Range(-_PositionRange.z, _PositionRange.z));
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(.3f,.3f,1);
            var from = transform.position - new Vector3(_PositionRange.x, 0, 0);
            var to = transform.position + new Vector3(_PositionRange.x, 0, 0);
            Gizmos.DrawLine(from, to);

            from = transform.position - new Vector3(0, _PositionRange.y, 0);
            to = transform.position + new Vector3(0, _PositionRange.y, 0);
            Gizmos.DrawLine(from, to);

            from = transform.position - new Vector3(0, 0, _PositionRange.z);
            to = transform.position + new Vector3(0, 0, _PositionRange.z);
            Gizmos.DrawLine(from, to);
        }
    }
}
