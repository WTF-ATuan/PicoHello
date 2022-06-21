using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Effect_Random : MonoBehaviour
{
    public GameObject[] Random_Effect = new GameObject[3];

    int i;

    private void OnEnable()
    {
        for( i=0; i<3; i++)
        {
            Random_Effect[i].SetActive(false);
        }

        i = Random.Range(0, 3);

        Random_Effect[i].SetActive(true);
    }
}
