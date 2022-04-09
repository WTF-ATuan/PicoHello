using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createBallScript : MonoBehaviour
{
    public bool isCreate;
    public float timer;
    float timeCount;
    public GameObject outPos;
    public GameObject insidePos;
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
        Vector3 p = Random.insideUnitSphere * outPos.transform.localScale.z;
        Vector3 pos = p.normalized * (insidePos.transform.localScale.z + p.magnitude);

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
