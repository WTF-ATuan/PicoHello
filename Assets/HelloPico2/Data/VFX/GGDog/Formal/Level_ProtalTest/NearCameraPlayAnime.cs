using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearCameraPlayAnime : MonoBehaviour
{
    public int NearCamera_Distance=100;
    public Animation[] Anime ;

    bool IsPlay = false;
    private void Update()
    {
        if(transform.position.z - Camera.main.transform.position.z< NearCamera_Distance && !IsPlay)
        {
            PlayAnime();
            IsPlay = true;
        }
    }



    [ContextMenu("PlayAnime")]
    public void PlayAnime()
    {
        for(int i=0;i<Anime.Length;i++)
        {
            Anime[i].Play();
        }

    }
}
