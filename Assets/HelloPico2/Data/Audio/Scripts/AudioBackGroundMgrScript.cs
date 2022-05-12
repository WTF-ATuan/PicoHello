using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBackGroundMgrScript : MonoBehaviour
{
    [Header("����BGM(�����}�C���ޭ�)")]
    public int index = 1;

    [Header("�V�����ܭ�(�L���_����) 0.025�̨�")]
    [Range(0.005f, 0.1f)]
    public float mix = 0.025f;

    
    public void Start()
    {
        BackgroundMusic_Mix.BGM_Mix.BGM_Mix_Play(index, mix);
    }
    
}
