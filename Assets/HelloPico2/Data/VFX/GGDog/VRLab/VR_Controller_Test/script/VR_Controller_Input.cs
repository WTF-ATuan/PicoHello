using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.PXR;

public class VR_Controller_Input : MonoBehaviour
{
    public XRController _controller;
    public Transform Hand_fixed;


    public GameObject[] Bullet;

    [Range(0, 1)]
    [Tooltip("�g���e���v")]
    public float FaultValue = 0.02f;


    [Tooltip("�o�g���j")]
    public float fireRate =0.35f;
    float nextFire;


    private void Awake()
    {
        //�e���P�w�����edot��
        Bullet[0].GetComponent<Bullet_Moving>().FaultValue = 1-FaultValue;
        Bullet[2].GetComponent<Bullet_Moving>().FaultValue = 1-FaultValue;
        Bullet[1].GetComponent<Bullet_Moving>().FaultValue = 1-FaultValue;
    }


    void Update()
    {
        //����(�e��)
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out var isTrigger);
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggetValue);

        //����(����)
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out var isGrip);
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue);
        _controller.inputDevice.TryGetFeatureValue(PXR_Usages.grip1DAxis, out var grip1DAxis);

        //Ĩۣ�Y
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out var padAxisTouch);
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var padAxisClick);
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue);

        //B��
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue);
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouchValue);

        //A��
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouchValue);

        /*
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            Bullet[0].GetComponent<Bullet_Moving>().Parent = Hand_fixed;
            GameObject bullet = Instantiate(Bullet[0], Hand_fixed);

        }*/

        if (isTrigger)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Debug.Log("isTrigger");

                Bullet[0].GetComponent<Bullet_Moving>().Parent = Hand_fixed;
                GameObject bullet = Instantiate(Bullet[0], Hand_fixed);


            }

        }


        if (isGrip)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Debug.Log("isGrip");

                Bullet[1].GetComponent<Bullet_Moving>().Parent = Hand_fixed;
                GameObject bullet = Instantiate(Bullet[1], Hand_fixed);

            }
        }

        if (primaryButtonValue)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Debug.Log("primaryButtonValue");

                Bullet[2].GetComponent<Bullet_Moving>().Parent = Hand_fixed;
                GameObject bullet = Instantiate(Bullet[2], Hand_fixed);

            }
        }

        if (secondaryButtonValue)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Debug.Log("secondaryButtonValue");

                Bullet[2].GetComponent<Bullet_Moving>().Parent = Hand_fixed;
                GameObject bullet = Instantiate(Bullet[2], Hand_fixed);

            }
        }


    }
}
