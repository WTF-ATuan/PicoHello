using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript_Jy : MonoBehaviour
{

    void Start()
    {
        var animator = GetComponentInChildren<Animator>();
        animator.Update(Random.value);
    }
}
