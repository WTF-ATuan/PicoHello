using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomShow : MonoBehaviour
{
    public GameObject[] objList;
    int randomObjNum;
    // Start is called before the first frame update
    void Start()
    {
        //randomObjNum = objList.Length + 1;
        int randomNum =  Random.Range(0, objList.Length);
        Debug.Log(randomNum);

        if (randomNum == 0)
        {
            objList[Random.Range(0, objList.Length)].SetActive(true);
            Debug.Log(1);
        }
        if(randomNum == 1)
        {
            foreach (GameObject objName in objList)
            {
                objName.SetActive(true);
            }
            Debug.Log(2);
        }
        
        /*
        foreach(GameObject objName in objList)
            {
                Debug.Log(objName);
            }    */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
