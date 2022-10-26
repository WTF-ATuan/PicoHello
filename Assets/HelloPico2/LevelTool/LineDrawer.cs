using HelloPico2.PlayerController.Arm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.PlayerController;
using Sirenix.OdinInspector;

namespace HelloPico2.LevelTool
{
    public class LineDrawer : MonoBehaviour
    {
        public LineRenderer _LineRenderer;
        public Transform _From;

        [FoldoutGroup("To Settings")] public ArmControllerInputMeshNameData _InputMeshData;
        [FoldoutGroup("To Settings")] public InputDevice.Scripts.HandType _ToHand;
        [FoldoutGroup("To Settings")] public ArmControllerInputMeshNameData.Controller _UseController;
        [FoldoutGroup("To Settings")] public ArmControllerInputMeshNameData.InputMesh _ShowOnThisInputMesh; 
        
        public Vector3 _FromOffset;
        public Vector3 _ToOffset;
        public Vector3 _CurveOffset;
        public int _Percision = 100;
        public AnimationCurve _Curve;
        ArmData armData;
        [ReadOnly][SerializeField] private Transform to;
        List<Vector3> points = new List<Vector3>();
        float dist;

        private void OnEnable()
        {
            var armDatas = GameObject.FindObjectsOfType<ArmData>();

            for (int i = 0; i < armDatas.Length; i++)
            {
                if (armDatas[i].HandType == _ToHand)
                    armData = armDatas[i];
            }

            _LineRenderer.positionCount = _Percision;
            
            if(_UseController == ArmControllerInputMeshNameData.Controller.Seperate)
                FindSeperateInputMesh();
            if (_UseController == ArmControllerInputMeshNameData.Controller.Combined)
                FindCombinedInputMesh();
        }
        private void FindSeperateInputMesh() {
            var name = _InputMeshData.GetMeshName(_UseController, _ShowOnThisInputMesh);

            var splitedName = name.Split('/');
            Transform searchThis = armData.transform;
            for (int i = 0; i < splitedName.Length; i++)
            {
                print(splitedName[i].ToString());
                searchThis = searchThis.Find(splitedName[i].ToString());
            }

            to = searchThis;
        }
        private void FindCombinedInputMesh()
        {
            var name = _InputMeshData.GetMeshName(_UseController, _ShowOnThisInputMesh);

            var splitedName = name.Split('/');
            Transform searchThis = armData.ArmorController.transform;
            for (int i = 0; i < splitedName.Length; i++)
            {
                print(splitedName[i].ToString());
                searchThis = searchThis.Find(splitedName[i].ToString());
            }
            to = searchThis;            
        }
        private void Update()
        {
            DrawLine();
        }
        private void DrawLine() {
            //to = armData.transform.position;

            points.Clear();
            dist = Vector3.Distance(_From.position + _FromOffset, to.position + _ToOffset);

            for (int i = 0; i < _Percision; i++)
            {
                var step = (i * dist / _Percision);
                var Pos = Vector3.Lerp(_From.position + _FromOffset, to.position + _ToOffset, step);
                var curve = Vector3.Lerp(Vector3.zero, _CurveOffset, _Curve.Evaluate(step));
                Pos = Pos + curve;
                points.Add(Pos);
            }
            
            _LineRenderer.SetPositions(points.ToArray());
        }
        private void GetToPosition() { 
            
        }
    }
}
