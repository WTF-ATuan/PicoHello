using UnityEngine;

public class CameraObliqueMatrix : MonoBehaviour
{

    public GameObject planeGO;

    public Camera c;

    private Vector4 clipPlane;
    private Plane plane;
    private Vector3[] vertices;
    private Mesh planeMesh;
    
    void Start()
    {
        planeMesh = planeGO.GetComponent<MeshFilter>().mesh;
    }
    
    void FixedUpdate()
    {
        vertices = planeMesh.vertices;

        for (int i = 0; i < 3; i++)
        {
            vertices[i] = planeGO.transform.TransformPoint(vertices[i]);
        }

        plane = new Plane(vertices[0], vertices[1], vertices[2]);
        
        Vector4 clipPlaneWorldSpace = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, -Vector3.Dot(plane.normal, planeGO.transform.position));
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(c.cameraToWorldMatrix) * -clipPlaneWorldSpace;

        c.projectionMatrix = c.CalculateObliqueMatrix(clipPlaneCameraSpace);

    }

}