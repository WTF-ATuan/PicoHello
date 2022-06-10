using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomShowScript : MonoBehaviour
{
    public GameObject[] ObjList;
    public GameObject[] PosList;
    int ObjLength;
    int PosLength;
    float createTime;
    // Start is called before the first frame update
    void Start()
    {
        ObjLength = ObjList.Length;
        PosLength = PosList.Length;

        InsGameObj();
    }
    void InsGameObj()
    {
            var InsObj = ObjList[Random.Range(0, ObjLength)];
            var InsPos = PosList[Random.Range(0, PosLength)];
            Instantiate(InsObj, InsPos.transform.position,InsObj.transform.localRotation,  gameObject.transform);

    }

}
