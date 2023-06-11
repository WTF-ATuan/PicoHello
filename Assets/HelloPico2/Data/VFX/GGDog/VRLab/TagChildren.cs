using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class TagChildren : MonoBehaviour
{
    private void OnEnable()
    {
        
        for(int i=0;i<transform.childCount;i++)
        {
            transform.GetChild(i).tag = "Hide";
        }

    }
}
