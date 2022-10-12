using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;

public class Anime_Play : MonoBehaviour
{

    [HideInInspector]
    public GameObject Pre_Level;
    [HideInInspector]
    public GameObject Next_Level;

    Animation anim;


    public GameObject Level_0to3;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    private void OnEnable()
    {
        anim.Play(anim.clip.name);
    }



    //�³����w����: ����
    void Level_Switch_Off()
    {
        Pre_Level.SetActive(false);
    }
    //�³����w����: �}��
    void Level_Switch_On()
    {
        Next_Level.SetActive(true);
    }

    //����0~3������
    void Level_0to3_Off()
    {
        Level_0to3.SetActive(false);
    }

    
    //�{��e��: On
    public void On_SeeThroughManual()
    {
        PXR_Boundary.EnableSeeThroughManual(true);
    }
    //�{��e��: Off
    public void Off_SeeThroughManual()
    {
        PXR_Boundary.EnableSeeThroughManual(false);
    }
}
