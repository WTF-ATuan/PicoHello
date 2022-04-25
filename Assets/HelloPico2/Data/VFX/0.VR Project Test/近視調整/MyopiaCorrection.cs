using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyopiaCorrection : MonoBehaviour {

    [Range(1,100)]
    public float Scale = 0.5f;


    GameObject[] AllChild = new GameObject[999];

    int ChildCount;
	void Awake ()
    {
        ChildCount = transform.childCount;
        for (int i = 0; i < ChildCount; i++)
        {
            AllChild[i] =  transform.GetChild(i).gameObject;
        }
    }
	
	void Update ()
    {
        transform.localScale = new Vector3(1, 1, 1);

        ParentNull();

        transform.position = new Vector3(0, 0, 0);

        transform.position = Camera.main.transform.position;

        ParentIn();
        transform.localScale = new Vector3(Scale, Scale, Scale);
        
    }

    //從此父物件解脫
    void ParentNull()
    {
        for (int i = 0; i < ChildCount; i++)
        {
            AllChild[i].transform.parent=null;
        }
    }

    //歸回此父物件
    void ParentIn()
    {
        for (int i = 0; i < ChildCount; i++)
        {
            AllChild[i].transform.parent = transform;
        }
    }
}
