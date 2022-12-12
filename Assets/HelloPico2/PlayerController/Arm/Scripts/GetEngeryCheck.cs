using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.PlayerController.Arm;
public class GetEngeryCheck : MonoBehaviour
{
    private ArmData _armData;
    private EnergyBallBehavior _EnergyBallBehavior;

    [SerializeField] private float slowShootEngery;
    [SerializeField] private float slowShootCoolDown;
    [SerializeField] private float DefaultShootCoolDown;
    // Start is called before the first frame update
    void Start()
    {
        _armData = GetComponent<ArmData>();
        _EnergyBallBehavior = GetComponent<EnergyBallBehavior>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (_armData.Energy < slowShootEngery)
        {
            _EnergyBallBehavior.SetShootCoolDown(slowShootCoolDown);
        }
        else
        {
            _EnergyBallBehavior.SetShootCoolDown(DefaultShootCoolDown);
        }
    }
}
