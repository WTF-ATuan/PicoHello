using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public GameObject ball;

    // VR Controller Obj
    public GameObject Left_Controller;
    public GameObject Right_Controller;

    // Toward Direction base in length threshold
    [Range(0, 1)]
    public float Direction_threshold;

    public AnimationCurve Move_Curve;
    [Range(0, 1)]
    public float Move_Curve_Speed = 0.25f;

    [Range(0, 1)]
    public float Move_Curve_Amplitude = 0.25f;

    

    // Get per Frame Speed
    Vector3 currentDir;
    Vector3 deltaDir;
    Vector3 lastDir = new Vector3(0, 0, 0);

    // Toward Direction
    Vector3 Dir = new Vector3(0, 0, 0);

    Vector3 Toward_Dir = new Vector3(0, 0, 0);
    
    private void Awake()
    {

        StartCoroutine(Curve_Toward());
    }


    bool Toward_Dir_fixed=false;

    void Update()
    {

        // caculate Toward Direction
       // Dir = ObjSpeed(Left_Controller) + ObjSpeed(Right_Controller);

        Dir = ObjSpeed(ball);

        // is greater than threshold => Active IEnumerator Curve_Toward()
        if (Direction_threshold < Dir.magnitude )
        {
            Debug.Log("Direction_threshold");

            Curve_Toward_Controller = true;  //Active Animation Curve

            if(!Toward_Dir_fixed)
            {
                Toward_Dir = -Dir/ Dir.magnitude;
                Toward_Dir_fixed = true;
            }
            StartCoroutine(Curve_Toward());


            //猛力划時，硬生生的再重頭跑一次Curve，危急逃生
            if( 0.5f < Dir.magnitude)
            {
                t = 0;

                if (!Toward_Dir_fixed)
                {
                    Toward_Dir = -Dir / Dir.magnitude;
                    Toward_Dir_fixed = true;
                }
            }

        }

    }


    Vector3 ObjSpeed(GameObject VRController)
    {
        currentDir = VRController.transform.position;
        deltaDir = currentDir - lastDir;
        lastDir = currentDir;
        return deltaDir;

    }

    bool Curve_Toward_Controller = false;

    float t = 0;
    IEnumerator Curve_Toward()
    {
        while (Curve_Toward_Controller && t <= 1)
        {
            yield return new WaitForSeconds(0);

            Debug.Log("Curve_Toward");

            t += Move_Curve_Speed * Time.deltaTime;

            transform.position = transform.position + Toward_Dir * Move_Curve.Evaluate(t) * Move_Curve_Amplitude * Time.deltaTime;

            if(t>0.5f)
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
