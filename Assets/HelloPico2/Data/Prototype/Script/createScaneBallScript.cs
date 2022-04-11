using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createScaneBallScript : MonoBehaviour
{
    public bool isCreate;
    public float timer;
    float timeCount;
    public GameObject outPos;
    public GameObject[] insObj;
    int randRange;

    // Start is called before the first frame update
    void Start()
    {
        randRange = insObj.Length;
        timeCount = timer;
    }
    void CreateObj()
    {
        Vector3 pos = new Vector3(outPos.transform.localPosition.x + Random.Range(-6, 6), outPos.transform.localPosition.y + Random.Range(-1, 5), outPos.transform.localPosition.z + Random.Range(-1, 3));

        Instantiate(insObj[Random.Range(0,randRange)], pos,Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
        timeCount = timeCount - Time.deltaTime;
        if (isCreate == true && timeCount<=0)
        {
            CreateObj();
            timeCount = timer;
        }
        
        
    }

}
