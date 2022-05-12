using HelloPico2.InputDevice.Scripts;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.XR.PXR;

public class usePicoHaptic : MonoBehaviour
{
    private ISelector _rightSelector;
    private ISelector _leftSelector;
    
    enum hapticType { reticle, select, fire, hit }
    public float _strength;
    public int _time;
    hapticType _hapticType;
    [SerializeField] float _hapticTime;
    //private ColdDownTimer _timer;
    
    float coldTime=3.0f;
    bool isSelect=false;
    
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
    public void OnTriggerStay(Collider other)
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
    }
    public void OnCollisionEnter(Collision collision)
    {
        if ( collision.gameObject.tag  == "Player")
        {
    
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
