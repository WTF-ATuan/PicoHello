using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBackGroundMgrScript : MonoBehaviour
{
    [Header("替換BGM(對應陣列索引值)")]
    public int index = 1;

    [Header("混音漸變值(無接縫→快) 0.025最佳")]
    [Range(0.005f, 0.1f)]
    public float mix = 0.025f;

    
    public void Start()
    {
        BackgroundMusic_Mix.BGM_Mix.BGM_Mix_Play(index, mix);
    }
    
}
