using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public GameObject ball;

    // VR Controller Obj
    public GameObject Left_Controller;
    public GameObject Right_Controller;

    public GameObject Swim_Dir_LC;
    public GameObject Swim_Dir_RC;

    // Toward Direction base in length threshold
    [Range(0, 1)]
    public float Direction_threshold;

    public AnimationCurve Move_Curve;
    [Range(0, 1)]
    public float Move_Curve_Speed = 0.25f;

    [Range(0, 1)]
    public float Move_Curve_Amplitude = 0.25f;

    [Range(0, 1)]
    public float Update_TowardDirInr = 0.25f;


    // Toward Direction
    Vector3 Dir = new Vector3(0, 0, 0);

    Vector3 Toward_Dir = new Vector3(0, 0, 1f);
    
    private void Awake()
    {

        StartCoroutine(Curve_Toward());
    }


    bool Toward_Dir_fixed=false;



    // Get per Frame Speed
    Vector3 LC_currentDir;
    Vector3 LC_deltaDir;
    Vector3 LC_lastDir = new Vector3(0, 0, 0);

    Vector3 RC_currentDir;
    Vector3 RC_deltaDir;
    Vector3 RC_lastDir = new Vector3(0, 0, 0);

    void Update()
    {

        // caculate Toward Direction
        // Dir = ObjSpeed(Left_Controller) + ObjSpeed(Right_Controller);

        // Dir = ObjSpeed(ball);

        LC_currentDir = Left_Controller.transform.localPosition;
        LC_deltaDir = LC_currentDir - LC_lastDir;
        LC_lastDir = LC_currentDir;

        RC_currentDir = Right_Controller.transform.localPosition;
        RC_deltaDir = RC_currentDir - RC_lastDir;
        RC_lastDir = RC_currentDir;

        Dir = LC_deltaDir * (Vector3.Dot(LC_deltaDir, Swim_Dir_LC.transform.forward) + 1) / 2
            + RC_deltaDir * (Vector3.Dot(RC_deltaDir, Swim_Dir_RC.transform.forward) + 1) / 2;

        // is greater than threshold => Active IEnumerator Curve_Toward()
        if (Direction_threshold < Dir.magnitude && Direction_threshold< LC_deltaDir.magnitude && Direction_threshold< RC_deltaDir.magnitude)
        {


            /*
            if(Vector3.Dot(Toward_Dir, -Dir / Dir.magnitude)<0)
            {
                Toward_Dir_fixed = true;
            }*/



            if(!Toward_Dir_fixed && Vector3.Dot(-Dir / Dir.magnitude, Camera.main.transform.forward)>0.25F)
            {
                Debug.Log(Dir);

                Curve_Toward_Controller = true;  //Active Animation Curve

                Dir = Vector3.Normalize(Dir);

                Toward_Dir = -Dir / Dir.magnitude;

                float TDotC = (Vector3.Dot(Toward_Dir, Camera.main.transform.forward)+1)/2;

                    //Toward_Dir = Toward_Dir * TDotC;
                    Toward_Dir_fixed = true;
                    t = 0;

                StartCoroutine(Curve_Toward());
            }

        }

    }

    /*
    Vector3 ObjSpeed(GameObject VRController)
    {
        currentDir = VRController.transform.localPosition;
        deltaDir = currentDir - lastDir;
        lastDir = currentDir;
        return deltaDir;
    }*/

    bool Curve_Toward_Controller = false;

    float t = 0;
    IEnumerator Curve_Toward()
    {
        while (Curve_Toward_Controller && t <= 1)
        {
            yield return new WaitForSeconds(0);

            //Debug.Log("Curve_Toward");

            t += Move_Curve_Speed*0.01F;

            transform.position = transform.position + Toward_Dir * Move_Curve.Evaluate(t) * Move_Curve_Amplitude * Time.deltaTime;

            if(Update_TowardDirInr < t )
            {
                Toward_Dir_fixed = false;
            }
        }
        yield return new WaitForSeconds(0);
        //InActive Animation Curve
        Curve_Toward_Controller = false;
        t = 0;
        Toward_Dir_fixed = false;
    }
}
