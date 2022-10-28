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
        public Follower _CameraFollower;
        public LineRenderer _LineRenderer;
        public Transform _From;

        [FoldoutGroup("To Settings")] public ArmControllerInputMeshNameData _InputMeshData;
        [FoldoutGroup("To Settings")] public InputDevice.Scripts.HandType _ToHand;
        [FoldoutGroup("To Settings")] public ArmControllerInputMeshNameData.Controller _UseController;
        [FoldoutGroup("To Settings")] public ArmControllerInputMeshNameData.InputMesh _ShowOnThisInputMesh; 
        
        public Vector3 _FromOffset;
        [Range(0,1f)]public float _ToOffsetStart = 0.8f;
        public Vector3 _ToOffset;
        public Vector3 _CurveOffset;
        public int _Percision = 100;
        public AnimationCurve _Curve;
        public AnimationCurve _ToOffsetCurve;
        public bool _EnableFindInputMeshDebber = false;
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
            
            _From.SetParent(transform);


            if (_UseController == ArmControllerInputMeshNameData.Controller.Seperate)
                FindSeperateInputMesh();
            if (_UseController == ArmControllerInputMeshNameData.Controller.Combined)
                FindCombinedInputMesh();

            _CameraFollower.Target = HelloPico2.Singleton.GameManagerHelloPico.Instance._MainCamera.transform;
        }
        private void FindSeperateInputMesh() {
            var name = _InputMeshData.GetMeshName(_UseController, _ShowOnThisInputMesh);

            var splitedName = name.Split('/');
            Transform searchThis = armData.transform;
            for (int i = 0; i < splitedName.Length; i++)
            {
                if(_EnableFindInputMeshDebber) print(splitedName[i].ToString());
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
                if(_EnableFindInputMeshDebber) print(splitedName[i].ToString());
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
            var toOffset = _ToOffset;
            for (int i = 0; i < _Percision; i++)
            {
                var step = (i * dist / _Percision);

                if (step > _ToOffsetStart)
                {
                    var approachingStep = (step - _ToOffsetStart) / (1 - _ToOffsetStart);
                    toOffset = Vector3.Lerp(_ToOffset, Vector3.zero, _ToOffsetCurve.Evaluate(approachingStep));
                }

                var Pos = Vector3.Lerp(_From.position + _FromOffset, to.position + toOffset, step);
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
