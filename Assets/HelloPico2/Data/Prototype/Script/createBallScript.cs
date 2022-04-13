using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createBallScript : MonoBehaviour
{
    public bool isCreate;
    public float timer;
    float timeCount;
    public GameObject[] createPos;
    public GameObject[] insObj;
    int randRange;
    int createRange;

    // Start is called before the first frame update
    void Start()
    {
        createRange = createPos.Length;
        randRange = insObj.Length;
        timeCount = timer;
        
    }
    void CreateObj()
    {
        GameObject parentName = GameObject.Find("BallPool");
        Vector3 insCreatePos = createPos[Random.Range(0, createRange)].transform.localPosition;
        Vector3 pos = new Vector3(insCreatePos.x + Random.Range(-3.5f, 3.5f), insCreatePos.y + Random.Range(-1, 3), insCreatePos.z + Random.Range(-1, 2));
        Instantiate(insObj[Random.Range(0,randRange)], pos,Quaternion.identity, parentName.transform);
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
