using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class LineSyncer : MonoBehaviour
    {
        public LineRenderer FollowThis;
        private LineRenderer lineRenderer;
        private Vector3[] positions;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = FollowThis.positionCount;
            positions = new Vector3[FollowThis.positionCount];
        }

        private void Update()
        {            
            FollowThis.GetPositions(positions);
            lineRenderer.SetPositions(positions);
        }
    }
}