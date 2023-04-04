using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class SortingLayer_FindChange : MonoBehaviour
{
    [SortingLayerTools.SortingLayer]
    public int SearchLayerID ;

    [SortingLayerTools.SortingLayer]
    public int AssignLayerID;
    

    public GameObject[] FindObject;


    void OnEnable()
    {
        ClearFindObject();
        FindChild(gameObject);

    }


    void FindChild(GameObject GO )
    {
        for (int i = 0; i < GO.transform.childCount; i++)
        {
            FindChild(GO.transform.GetChild(i).gameObject);

            if (GO.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>())
            {
                if (GO.transform.GetChild(i).gameObject.GetComponent<ParticleSystemRenderer>().sortingLayerID == SearchLayerID)
                {
                    Debug.Log(GO.transform.GetChild(i).gameObject.name);

                    AddFindObject(GO.transform.GetChild(i).gameObject);
                    
                }
            }

        }
    }

    //添加至陣列
    void AddFindObject( GameObject GO )
    {
        for(int i=0; i<FindObject.Length;i++)
        {
           if( FindObject[i] == null)
            {
                FindObject[i] = GO;
                break;
            }
        }
    }

    //找尋指定Layer物件並置於FindObject陣列
    [ContextMenu("[Searching]")]
    void SearchingLayerObject()
    {
        ClearFindObject();
        FindChild(gameObject);
        
        int find=0;
        for (int i = 0; i < FindObject.Length; i++)
        {
            if (FindObject[i]==null)
            {
                find++;
            }
        }
        if(find == FindObject.Length)
        {
            Debug.Log("Searching has been finished ,no objects match the SearchLayerID ");
        }
    }

    //指定所有選中物件轉為指定的Layer
    [ContextMenu("[AssignLayer]")]
    void AssignLayer()
    {
        if (FindObject[0])
        {
            for (int i = 0; i < FindObject.Length; i++)
            {
                if (FindObject[i])
                {
                    FindObject[i].gameObject.GetComponent<ParticleSystemRenderer>().sortingLayerID = AssignLayerID;
                }
            }
            Debug.Log("Finish assign SortingLayer ~");
        }

        else
        {
            Debug.LogAssertion("No Objects in array ,please [Searching] first!");
        }

    }


    //清空陣列
    [ContextMenu("[Clear]")]
    void ClearFindObject()
    {
        for (int i = 0; i < FindObject.Length; i++)
        {
            if (FindObject[i])
            {
                FindObject[i] = null;
            }
        }
    }


}
