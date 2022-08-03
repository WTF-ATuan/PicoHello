using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimaKey_CloseObj : MonoBehaviour
{
    public GameObject[] Obj;
    public void CloseObj()
    {
        for(int i=0; i< Obj.Length; i++)
        {
            Obj[i].SetActive(false);
        }

    }
}
