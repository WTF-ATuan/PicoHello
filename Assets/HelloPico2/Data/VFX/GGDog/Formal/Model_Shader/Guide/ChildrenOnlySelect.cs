using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChildrenOnlySelect : MonoBehaviour
{
    [Range(0,37)]
    public int I = 0;
    void Update()
    {
        for(int i=0;i<transform.childCount;i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        transform.GetChild(I).gameObject.SetActive(true);
    }
}
