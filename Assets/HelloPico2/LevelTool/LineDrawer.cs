using HelloPico2.PlayerController.Arm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class LineDrawer : MonoBehaviour
    {
        public LineRenderer _LineRenderer;
        public Transform _From;
        public InputDevice.Scripts.HandType _ToHand;        
        public Vector3 _FromOffset;
        public Vector3 _CurveOffset;
        public int _Percision = 100;
        public AnimationCurve _Curve;
        ArmData armData; 
        public List<Vector3> points = new List<Vector3>();
        float dist;

        private void Awake()
        {
            var armDatas = GameObject.FindObjectsOfType<ArmData>();

            for (int i = 0; i < armDatas.Length; i++)
            {
                if (armDatas[i].HandType == _ToHand)
                    armData = armDatas[i];
            }

            _LineRenderer.positionCount = _Percision;
            //_LineRenderer.transform.position = Vector3.zero;
        }
        private void Update()
        {
            DrawLine();
        }
        private void DrawLine() {
            points.Clear();
            dist = Vector3.Distance(_From.position + _FromOffset, armData.transform.position);

            for (int i = 0; i < _Percision; i++)
            {
                var step = (i * dist / _Percision);
                var Pos = Vector3.Lerp(_From.position + _FromOffset, armData.transform.position, step);
                var curve = Vector3.Lerp(Vector3.zero, _CurveOffset, _Curve.Evaluate(step));
                Pos = Pos + curve;
                points.Add(Pos);
            }
            
            _LineRenderer.SetPositions(points.ToArray());
        }
    }
}
