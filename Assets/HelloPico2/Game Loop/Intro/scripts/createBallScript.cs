using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class createBallScript : MonoBehaviour
{
    public bool isCreate;
    public float timer;
    float timeCount;
    public GameObject[] createPos;
    public GameObject[] insObj;
    public GameObject SetGroup;
    public int createType = 0;//0 local place ,1 toward player,2 enemy Type1 
    int randRange;
    int createRange;
    public bool isMulit;
    public Vector3[] rangSize;
    public  GameObject setingPool;

    [BoxGroup("NEW")]
    public float fadeinDuring = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        createRange = createPos.Length;
        randRange = insObj.Length;
        timeCount = timer;
        

    }
    public void CreateMultiObj()
    {
        Vector3 insCreatePos = createPos[Random.Range(0, createRange)].transform.localPosition;
        GameObject parentName = setingPool;
        Vector3 pos = new Vector3(insCreatePos.x + Random.Range(rangSize[0][0], rangSize[1][0]), insCreatePos.y + Random.Range(rangSize[0][1], rangSize[1][1]), insCreatePos.z + Random.Range(rangSize[0][2], rangSize[1][2]));
        GameObject getGroup = Instantiate(SetGroup, Vector3.zero, Quaternion.identity, parentName.transform);
        var obj = Instantiate(insObj[Random.Range(0, randRange)], pos, insObj[0].transform.localRotation, getGroup.transform);
        //Scale By Benson
        var originScale = obj.transform.localScale;
        obj.transform.DOScale(Vector3.zero, 0); //將其物件先設成最小Scale;
        obj.transform.DOScale(originScale, fadeinDuring).SetEase(Ease.Linear);
        //Scale By Benson
    }

    public void CreateObj()
    {
        
        Vector3 insCreatePos = createPos[Random.Range(0, createRange)].transform.localPosition;
        GameObject parentName = setingPool;
        //Destroy(parentName.transform.GetChild(0).gameObject);
        GameObject getGroup = Instantiate(SetGroup, Vector3.zero, Quaternion.identity, parentName.transform);
        Vector3 pos = new Vector3(insCreatePos.x + Random.Range(rangSize[0][0], rangSize[1][0]), insCreatePos.y + Random.Range(rangSize[0][1], rangSize[1][1]), insCreatePos.z + Random.Range(rangSize[0][2], rangSize[1][2]));
        var obj = Instantiate(insObj[Random.Range(0, randRange)], pos, Quaternion.identity, getGroup.transform);
        //Scale By Benson
        var originScale = obj.transform.localScale;
        obj.transform.DOScale(Vector3.zero, 0); //將其物件先設成最小Scale;
        obj.transform.DOScale(originScale, fadeinDuring).SetEase(Ease.Linear);
        //Scale By Benson
        obj.SetActive(true);
        var destroy = obj.AddComponent<DestroyObject>();
        destroy.enabled = true;
        destroy.DestoryTime = 3;
        /*if (parentName.transform.childCount != 0)
        {
            Destroy(parentName.transform.GetChild(0).gameObject);
            GameObject getGroup = Instantiate(SetGroup, Vector3.zero, Quaternion.identity, parentName.transform);
            Vector3 pos = new Vector3(insCreatePos.x + Random.Range(rangSize[0][0], rangSize[1][0]), insCreatePos.y + Random.Range(rangSize[0][1], rangSize[1][1]), insCreatePos.z + Random.Range(rangSize[0][2], rangSize[1][2]));
            Instantiate(insObj[Random.Range(0, randRange)], pos, Quaternion.identity, getGroup.transform);
        }*/
        /*
        if (createType == 0)
        {
            GameObject parentName = GameObject.Find("BallPool");
            Vector3 pos = new Vector3(insCreatePos.x + Random.Range(-3.5f, 3.5f), insCreatePos.y + Random.Range(-1, 3), insCreatePos.z + Random.Range(-1, 2));
            Instantiate(insObj[Random.Range(0, randRange)], pos, Quaternion.identity, parentName.transform);
        }
        else if(createType == 1)
        {
            GameObject parentName = setingPool;

            if (parentName.transform.childCount != 0)
            {
                Destroy(parentName.transform.GetChild(0).gameObject);
                GameObject getGroup = Instantiate(SetGroup, Vector3.zero, Quaternion.identity, parentName.transform);
                Vector3 pos = new Vector3(insCreatePos.x + Random.Range(rangSize[0][0], rangSize[1][0]), insCreatePos.y + Random.Range(rangSize[0][1], rangSize[1][1]), insCreatePos.z + Random.Range(rangSize[0][2], rangSize[1][2]));
                Instantiate(insObj[Random.Range(0, randRange)], pos, Quaternion.identity, getGroup.transform);
            }

            
            //moveGrp.SetActive(true);
        }
        
        else
        {
            GameObject parentName = setingPool;
            Vector3 pos = new Vector3(insCreatePos.x + Random.Range(rangSize[0][0], rangSize[1][0]), insCreatePos.y + Random.Range(rangSize[0][1], rangSize[1][1]), insCreatePos.z + Random.Range(rangSize[0][2], rangSize[1][2]));
            GameObject getGroup = Instantiate(SetGroup,Vector3.zero,Quaternion.identity, parentName.transform);
            Instantiate(insObj[Random.Range(0, randRange)], pos, insObj[0].transform.localRotation, getGroup.transform);
        }
            */


    }
    // Update is called once per frame
    void Update()
    {
        if(createType !=1 && timer != 999)
        {
            timeCount = timeCount - Time.deltaTime;
            if (isCreate == true && timeCount <= 0 )
            {
                if (isMulit)
                {
                    CreateMultiObj();
                    timeCount = timer;
                }
                else 
                {
                    CreateObj();
                    timeCount = timer;
                }

            }
        }
    }

}
