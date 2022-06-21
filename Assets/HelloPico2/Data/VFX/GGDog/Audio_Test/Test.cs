using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Header("替換BGM(對應陣列索引值)")]
    public int index = 1;

    [Header("混音漸變值(無接縫→快) 0.025最佳")]
    [Range(0.005f,0.1f)]
    public float mix = 0.025f;
    
    //兩首漸變混播放，第一首直接播放則不會有淡入
    [ContextMenu("***BGM_Mix_Play***")]
    public void BGM_Mix_Play()
    {
        BackgroundMusic_Mix.BGM_Mix.BGM_Mix_Play(index, mix);
    }
}


