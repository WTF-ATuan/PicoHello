using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererDrawer : MonoBehaviour
{
    public LineRenderer _LineRenderer;
    public Transform _From;
    public Transform _To;
    private int positionCount= 2;
    List<Vector3> points = new List<Vector3>();

    [Header("Curve")]
    public bool _UseCurve;
    public Vector3 _FromOffset;
    [Range(0, 1f)] public float _ToOffsetStart = 0.8f;
    public Vector3 _ToOffset;
    public Vector3 _CurveOffset;
    public int _Percision = 100;
    public AnimationCurve _Curve;
    public AnimationCurve _ToOffsetCurve;
    public float _StartFlattenCurveDist;

    float dist;

    private void Awake()
    {
        if (_UseCurve)        
            _LineRenderer.positionCount = _Percision;        
        else 
            _LineRenderer.positionCount = positionCount;            
        
        for (int i = 0; i < _LineRenderer.positionCount; i++)
        {
            points.Add(Vector3.zero);
        }
    }
    private void Update()
    {
        if (_From == null || _To == null) return;

        if (_UseCurve)
            DrawCurve();
        else
            DrawLine();
    }
    private void DrawLine() {
        points[0] = _From.position;
        points[1] = _To.position;

        _LineRenderer.SetPositions(points.ToArray());
    }
    private void DrawCurve()
    {
        points.Clear();
        var from = _From.TransformPoint(_FromOffset);
        dist = Vector3.Distance(from, _To.position + _ToOffset);
        
        var toOffset = _ToOffset;
        //print(dist - _StartFlattenCurveDist);
        var curveOffset = Vector3.Lerp(Vector3.zero, _CurveOffset, Mathf.Clamp(dist - _StartFlattenCurveDist, 0, 1));

        for (int i = 0; i < _Percision; i++)
        {
            var step = Mathf.Clamp(i * (dist / _Percision), 0, 1);

            if(i + 1 == _Percision) step = 1;

            var Pos = Vector3.Lerp(from, _To.position + toOffset, step);
            var curve = Vector3.Lerp(Vector3.zero, curveOffset, _Curve.Evaluate(step));
            Pos = Pos + curve;
            points.Add(Pos);
        }

        _LineRenderer.SetPositions(points.ToArray());
    }
}
