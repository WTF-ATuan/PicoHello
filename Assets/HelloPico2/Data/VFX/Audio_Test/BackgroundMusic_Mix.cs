using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic_Mix : MonoBehaviour
{
    [Header("整體音量")]
    [Range(0, 1)]
    public float BGM_Volume = 1;

    [Header("此放置所有BGM")]
    public AudioClip[] BackgroundMusic_Source = new AudioClip[10];
    


    AudioSource BGM_Previous;
    AudioSource BGM_Next;

    static public BackgroundMusic_Mix BGM_Mix;

    void Start()
    {
        BGM_Mix = GetComponent<BackgroundMusic_Mix>();

        BGM_Previous = gameObject.AddComponent<AudioSource>();
        BGM_Next = gameObject.AddComponent<AudioSource>();

        BGM_Previous.loop = true;
        BGM_Next.loop = true;

        BGM_Next.clip = BackgroundMusic_Source[0];
        BGM_Next.volume = 1;
    }
    
    bool BGM_Volume_Adjust = true;

    private void Update()
    {
        //整體音量調整
        if (BGM_Volume_Adjust ) { BGM_Next.volume = BGM_Volume; }
    }

    //兩首漸變混播放，第一首直接播放則不會有淡入
    [ContextMenu("***BGM_Mix_Play***")]
    public void BGM_Mix_Play(int index,float mix)
    {
        BGM_Volume_Adjust = false;
        StartCoroutine(Mixing(index, mix));
    }

    IEnumerator Mixing(int index, float mix)
    {
        BGM_Previous.volume = BGM_Next.volume;

        BGM_Next.clip = BackgroundMusic_Source[index];
        BGM_Next.volume = 0.01f;
        BGM_Next.Play();

        while (BGM_Previous.volume>0.025f)
        {
            BGM_Previous.volume -= mix;
            BGM_Next.volume += mix;
            yield return new WaitForSeconds(0.1f);  
        }

        BGM_Previous.clip = BGM_Next.clip;
        BGM_Previous.time = BGM_Next.time;
        BGM_Previous.volume = 0;
        BGM_Previous.Play();

        BGM_Next.volume = 1;

        StopCoroutine(Mixing(index, mix));
        BGM_Volume_Adjust = true;
        yield return null;
    }
}
