using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Ball_Spawner))]

[ExecuteInEditMode]
public class Ball_Spawner_Range : MonoBehaviour
{

    public float Range_X = 1;
    public float Range_Y = 1;
    public float Range_Z = 1;

    public Color RangeLineColor = new Color(0.75f, 1, 0.5f, 0.5f);

    void Update()
    {
        Vector3 Pos = transform.localPosition;
        
        Debug.DrawRay(Pos + new Vector3(-Range_X, Range_Y, -Range_Z), new Vector3(2 * Range_X, 0, 0) , RangeLineColor);
        Debug.DrawRay(Pos + new Vector3(-Range_X, -Range_Y, -Range_Z), new Vector3(2 * Range_X, 0, 0), RangeLineColor);
        Debug.DrawRay(Pos + new Vector3(-Range_X, Range_Y, Range_Z), new Vector3(2 * Range_X, 0, 0), RangeLineColor);
        Debug.DrawRay(Pos + new Vector3(-Range_X, -Range_Y, Range_Z), new Vector3(2 * Range_X, 0, 0), RangeLineColor);
        
        Debug.DrawRay(Pos + new Vector3(-Range_X, Range_Y, -Range_Z), new Vector3(0, -2 * Range_Y, 0), RangeLineColor);
        Debug.DrawRay(Pos + new Vector3(Range_X, Range_Y, -Range_Z), new Vector3(0, -2 * Range_Y, 0), RangeLineColor);
        Debug.DrawRay(Pos + new Vector3(-Range_X, Range_Y, Range_Z), new Vector3(0, -2 * Range_Y, 0), RangeLineColor);
        Debug.DrawRay(Pos + new Vector3(Range_X, Range_Y, Range_Z), new Vector3(0, -2 * Range_Y, 0), RangeLineColor);

        Debug.DrawRay(Pos + new Vector3(-Range_X, Range_Y, -Range_Z), new Vector3(0, 0, 2 * Range_Z), RangeLineColor);
        Debug.DrawRay(Pos + new Vector3(Range_X, Range_Y, -Range_Z), new Vector3(0, 0, 2 * Range_Z), RangeLineColor);
        Debug.DrawRay(Pos + new Vector3(-Range_X, -Range_Y, -Range_Z), new Vector3(0, 0, 2 * Range_Z), RangeLineColor);
        Debug.DrawRay(Pos + new Vector3(Range_X, -Range_Y, -Range_Z), new Vector3(0, 0, 2 * Range_Z), RangeLineColor);
    }
}
