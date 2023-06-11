using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedGlow : MonoBehaviour
{
    public float MaxScale = 1;

    [Range(0,0.1f)]
    public float LowestSpeed = 0.03f;

    public float ScaleStep = 25;

    Vector3 currentDir;
    Vector3 deltaDir;
    Vector3 lastDir = new Vector3(0, 0, 0);

    void Update()
    {
        currentDir = transform.position;
        deltaDir = currentDir - lastDir;
        lastDir = currentDir;

        deltaDir = Vector3.ClampMagnitude(deltaDir, 1);

        float speed = Vector3.Magnitude(deltaDir);

        speed = Mathf.Max((speed - LowestSpeed) / (1 - LowestSpeed),0);

        speed = Mathf.Min(speed * ScaleStep , MaxScale);

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * speed, 1);
        

    }
}
