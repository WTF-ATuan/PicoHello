using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(LineRenderer))][ExecuteInEditMode()]
public class TrajectoryMover : MonoBehaviour
{
    public HelloPico2.LevelTool.BaseSpawner _Spawner;
    public LineRenderer _LineRenderer;
    [Range(20,999)]public int _LineSegmentCount = 20;
    public float dist;
    [ReadOnly]public List<Vector3> _LinePoints = new List<Vector3>();
    private void OnEnable()
    {
        UpdateTrajectory(_Spawner);        
    }
    private void OnValidate()
    {
        if (_Spawner != null) _Spawner.Notify = UpdateTrajectory;
        UpdateTrajectory(_Spawner);
    }
    float stepTime;
    float tuner = 0.148f;
    private void Update()
    {
        if (transform.hasChanged) UpdateTrajectory(_Spawner);
    }
    private void UpdateTrajectory(HelloPico2.LevelTool.BaseSpawner spawner) {
        if (spawner._Force == 0) return;

        Vector3 velocity = _Spawner.transform.forward * _Spawner._Speed * tuner;

        float FlightDuration = Mathf.Abs((2 * velocity.y) /_Spawner._Force);

        stepTime = FlightDuration / _LineSegmentCount;

        _LinePoints.Clear();

        for (int i = 0; i < _LineSegmentCount; i++) 
        {
            //float stepTimePassed = stepTime * i;
            float stepTimePassed = Time.fixedDeltaTime * i * dist;

            Vector3 MovementVector = velocity * stepTimePassed;
            MovementVector += _Spawner._ForceDir * (_Spawner._Force / 2) * Mathf.Pow(stepTimePassed, 2);

            _LinePoints.Add(spawner.transform.position + MovementVector * spawner._SpeedMultiplier);
        }

        _LineRenderer.positionCount = _LinePoints.Count;
        _LineRenderer.SetPositions(_LinePoints.ToArray());
    }
}
