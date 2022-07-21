using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetEnableObj_Random : MonoBehaviour
{
    public GameObject[] Random_Effect ;

    int i;

    private void OnEnable()
    {
        for( i=0; i< Random_Effect.Length; i++)
        {
            Random_Effect[i].SetActive(false);
        }

        i = Random.Range(0, Random_Effect.Length);

        Random_Effect[i].SetActive(true);
    }
}
