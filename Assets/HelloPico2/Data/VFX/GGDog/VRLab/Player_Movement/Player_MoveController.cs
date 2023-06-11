using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Player_MoveController : MonoBehaviour
{

    public Color32 RayCol;
    public Color32 RaySphereCol;
    public Vector3 Dir = new Vector3(1,1,1);
    public float RayLength = 1;

    public float RaySphereR = 0.025f;

    void Start()
    {
        
    }

    void OnDrawGizmosSelected()
    {
       // Gizmos.color = RaySphereCol;
       // Debug.DrawWireCapsule(transform.position, transform.position + Dir * RayLength, 0.1f);
       // Debug.DrawWireCapsule(transform.position, transform.position + new Vector3(-Dir.x, Dir.y, Dir.z) * RayLength, 0.1f);

        Dir = Vector3.Normalize(Dir);
        Debug.DrawLine(transform.position, transform.position + Dir * RayLength, RayCol);
        Debug.DrawLine(transform.position, transform.position + new Vector3(-Dir.x, Dir.y, Dir.z) * RayLength, RayCol);

        Debug.DrawSphere(transform.position, RaySphereR, RaySphereCol);

        Debug.DrawSphere(transform.position + Dir * RayLength, RaySphereR, RaySphereCol);
        Debug.DrawSphere(transform.position + new Vector3(-Dir.x, Dir.y, Dir.z) * RayLength, RaySphereR, RaySphereCol);


    }

    void Update()
    {
    }
}
