using HelloPico2.InputDevice.Scripts;
using Project;
using UnityEngine;
using Unity.XR.PXR;
using HandType = HelloPico2.InputDevice.Scripts.HandType;

public class usePicoHaptic : MonoBehaviour
{
    private ISelector _rightSelector;
    private ISelector _leftSelector;
    
    enum hapticType { reticle, select, fire, hit }
    public float _strength;
    public int _time;
    hapticType _hapticType;
    //[SerializeField] float _hapticTime;
    //private ColdDownTimer _timer;
    bool isSelect = false;
    float coldTime=3.0f;
    public bool isReticle;
    
    /*private void Start()
    {
        _timer = new ColdDownTimer(_hapticTime);
    }*/
    private void Start()
    {
        EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
        
    }
    private void OnDeviceInputDetected(DeviceInputDetected obj)
    {
        //判定是否是右手 是的話就抓取 
        if (obj.Selector.HandType == HandType.Right)
        {
            _rightSelector = obj.Selector;
        }

        if (obj.Selector.HandType == HandType.Left)
        {
            _leftSelector = obj.Selector;
        }
    }
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
            isSelect = false;
        }
        else
        {
            PXR_Input.SetControllerVibration(strength, time, PXR_Input.Controller.LeftController);
            isSelect = false;
        }
    }


    public void StartHapticsHit()
    {
         PicoHaptic(false, 0.5f, 100);
    }


    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTrigger:" + other.name);
        //Debug.Log(this.name);

        if (isReticle)
        {
            if (this.name == "Reticle_R")
            {
                //Debug.Log("Reticle_R");

                PicoHaptic(true, 0.3f, 100);
            }
            else 
            {
                PicoHaptic(false, 0.3f, 100);
                //Debug.Log("Reticle_L");
            }
        }
        else
        {
            if (other.tag == "RightHand Controller")
            {
                Debug.Log("getRight");
                //PicoHaptic(true, _strength, _time);
                PicoHaptic(true, 0.3f, 100);

            }
            if(other.tag == "LeftHand Controller")
            {
                PicoHaptic(false, 0.3f, 100);
                //PicoHaptic(false, _strength, _time);
                Debug.Log("LeftHand Controller");
            }
        }

        
    }
    /*
    public void OnCollisionEnter(Collision collision)
    {
        if ( collision.gameObject.tag  == "Player")
        {
    
            if (collision.gameObject.name == "ArmConAnimRPrefab")
            {
                hapticList(true);
            }
            else
            {
                hapticList(false);
            }
        }
    }*/
}
