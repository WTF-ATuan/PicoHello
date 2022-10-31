using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_FarUp : MonoBehaviour
{
    public float Range_X=3.5F;
    public float Range_Y=1;
    [Range(0,1)]
    public float Lerp = 0.1F;

    public GameObject[] GO;

    Vector3[] GO_OriPos = new Vector3[100];

    private void Awake()
    {
        for (int i = 0; i < GO.Length; i++)
        {
            GO_OriPos[i]= GO[i].transform.position ;
        }
    }


    void Update()
    {

        float CameraDot = Vector3.Dot(Vector3.Normalize(Camera.main.transform.TransformDirection(Vector3.forward)), Vector3.forward);

        Vector2 Camera_Forward = Camera.main.transform.TransformDirection(Vector3.forward);
        Vector2 Camera_Up = Camera.main.transform.TransformDirection(Vector3.up);

        if (Camera_Forward.y<0)
        {
            Camera_Forward.y = 0;
        }


        for (int i = 0; i < GO.Length; i++)
        {

            Vector3 Camera_Move = Vector3.Lerp(GO_OriPos[i], new Vector3(-(Camera_Forward.x + Camera_Up.x) * Range_X, -Camera_Forward.y * Range_Y, 0), 0.5f * (1 - CameraDot / 2));

            GO[i].transform.position = Vector3.Lerp(GO[i].transform.position, Camera_Move, Lerp) ;
        }

    }


}


