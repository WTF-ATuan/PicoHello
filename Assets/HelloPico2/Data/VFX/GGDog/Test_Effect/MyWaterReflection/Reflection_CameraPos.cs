using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflection_CameraPos : MonoBehaviour
{

    public RenderTexture _RenderTexture;


    GameObject CameraPos_Reflection;
    
    GameObject CameraPos;





    Camera Camera_Reflection;

    private Vector4 clipPlane;
    private Plane plane;
    private Vector3[] vertices;
    private Mesh planeMesh;

    
    void Start()
    {
        CameraPos = new GameObject();

        //創造第二個鏡頭
        CameraPos_Reflection = new GameObject(); CameraPos_Reflection.name = "Camera_Reflection";
        Camera_Reflection = CameraPos_Reflection.AddComponent<Camera>();
        Camera_Reflection.targetTexture = _RenderTexture;
        Camera_Reflection.cullingMask = ~(1 << 3); //用來做不顯示玩家倒影的Layer，這裡的Player層設在第3層

        planeMesh = GetComponent<MeshFilter>().mesh;
    }


    void LateUpdate()
    {


        transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-90, 0, -180)); //因為Plane跟Quad的軸向不同要轉
        
        CameraPos.transform.position = Camera.main.transform.position;
        CameraPos.transform.rotation = Camera.main.transform.rotation;

        CameraPos.transform.parent = transform;
        CameraPos_Reflection.transform.parent = transform;
        

        CameraPos_Reflection.transform.localPosition = new Vector3(CameraPos.transform.localPosition.x, -CameraPos.transform.localPosition.y, CameraPos.transform.localPosition.z);

        CameraPos_Reflection.transform.localRotation = new Quaternion(-CameraPos.transform.localRotation.x, CameraPos.transform.localRotation.y, -CameraPos.transform.localRotation.z, CameraPos.transform.localRotation.w);
        

        CameraPos_Reflection.transform.parent = null;
        CameraPos.transform.parent = null;

        transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(90, 0, 180));


        /*********************************/
        vertices = planeMesh.vertices;

        for (int i = 0; i < 3; i++)
        {
            vertices[i] = transform.TransformPoint(vertices[i]);
        }
        plane = new Plane(vertices[0], vertices[1], vertices[2]);

        Vector4 clipPlaneWorldSpace = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, -Vector3.Dot(plane.normal, transform.position));
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Camera_Reflection.cameraToWorldMatrix) * -clipPlaneWorldSpace;

        Camera_Reflection.projectionMatrix = Camera_Reflection.CalculateObliqueMatrix(clipPlaneCameraSpace);

        /*********************************/


    }

    

}
