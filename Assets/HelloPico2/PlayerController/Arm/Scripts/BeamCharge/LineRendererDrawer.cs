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
    private void Awake()
    {
        _LineRenderer.positionCount = positionCount;
        
        for (int i = 0; i < positionCount; i++)
        {
            points.Add(Vector3.zero);
        }
    }
    private void Update()
    {
        if (_From == null || _To == null) return;

        points[0] = _From.position;
        points[1] = _To.position;
        
        _LineRenderer.SetPositions(points.ToArray());
    }
}
