using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.PXR;

public class usePicoHaptic : MonoBehaviour
{
    enum hapticType { reticle, select, fire, hit }
    public float _strength;
    public int _time;
    hapticType _hapticType;
    


    private void hapticList(bool hapticHand)
    {
        
        switch (_hapticType)
        {
            case hapticType.reticle:
                PicoHaptic(hapticHand, _strength, _time);
                break;
            case hapticType.select:
                PicoHaptic(hapticHand, _strength, _time);
                break;
            case hapticType.fire:
                PicoHaptic(hapticHand, _strength, _time);
                break;
            case hapticType.hit:
                PicoHaptic(hapticHand, _strength, _time);
                break;
        }
    }


    public void PicoHaptic(bool touchHand, float strength, int time)
    {
        if (touchHand)
        {
            PXR_Input.SetControllerVibration(strength, time, PXR_Input.Controller.RightController);
        }
        else
        {
            PXR_Input.SetControllerVibration(strength, time, PXR_Input.Controller.LeftController);
        }
    }


    /*public void StartHapticsHit()
    {
         PicoHaptic(false, 0.5f, 100);
    }*/


    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTrigger:" + other.name);
        if (other.tag == "Player")
        {
            
            //var getController = other.GetComponent<XRController>().controllerNode;
            if (other.name == "RightHand Controller")
            {
                hapticList(true);
            }
            else
            {
                hapticList(false);
            }

        }
        else if (other.tag == "reticle")
        {
            if(other.name == "reticleRPrefab")
            {
                PicoHaptic(true,0.3f,100);
            }
            else
            {
                PicoHaptic(false, 0.3f, 100);
            }
        }
    }
    /*public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.name == "RHand(Clone)")
            {
                hapticList(true);
            }
            else
            {
                hapticList(false);
            }
        }
    }*/
    public void OnCollisionEnter(Collision collision)
    {
        if ( collision.gameObject.tag  == "Player")
        {
            Debug.Log("collision:" + collision.gameObject.name);
            //var getController = other.GetComponent<XRController>().controllerNode;
            if (collision.gameObject.name == "PropArmLPrefab")
            {
                hapticList(true);
            }
            else
            {
                hapticList(false);
            }

        }
    }
}
