using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anime_Play : MonoBehaviour
{

    public Animation anim;


    private void OnEnable()
    {
        anim.Play("Boss_Explosion");
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
