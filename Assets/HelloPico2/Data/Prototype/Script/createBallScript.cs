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
    public int createType = 0;//0 local place ,1 toward player
    int randRange;
    int createRange;
    public Vector3[] rangSize;
    public GameObject moveBall;

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

        if (createType == 0)
        {
            Vector3 pos = new Vector3(insCreatePos.x + Random.Range(-3.5f, 3.5f), insCreatePos.y + Random.Range(-1, 3), insCreatePos.z + Random.Range(-1, 2));
            Instantiate(insObj[Random.Range(0, randRange)], pos, Quaternion.identity, parentName.transform);
        }
        else
        {
            Vector3 pos = new Vector3(insCreatePos.x + Random.Range(rangSize[0][0], rangSize[1][0]), insCreatePos.y + Random.Range(rangSize[0][1], rangSize[1][1]), insCreatePos.z + Random.Range(rangSize[0][2], rangSize[1][2]));
            GameObject moveGrp = Instantiate(moveBall, pos, Quaternion.identity, parentName.transform);
            Instantiate(insObj[Random.Range(0, randRange)], pos, Quaternion.identity, moveGrp.transform);
            moveGrp.SetActive(true);
        }
            

        
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
