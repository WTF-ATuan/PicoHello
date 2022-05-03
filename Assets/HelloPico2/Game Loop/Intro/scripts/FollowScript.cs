using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    public Transform player;

    public bool isGuideCheck =true;
    public bool isLook=false;
    public bool isCheckMove;
    private string animtionName;
    public float waitTimeValue;
    Animator m_Animator;
    public Transform[] tragetMovePos;
    int listLength;
    int isCount = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        listLength = tragetMovePos.Length;
        m_Animator = GetComponent<Animator>();
        isCheckMove = true;
        
    }
    /*
    void CaseTypeCheck()
    {
        switch (caseType)
        {
            case 0:
                Debug.Log("case 0");
                break;
        }
    }*/
    // Update is called once per frame

    public  void FixedUpdate()
    {

        /*
        if (caseOne[0] == null && caseOne[1] == null)
        {
            isGuideCheck = true;
        }*/
        //movePos(isCheckMove);
        if (isGuideCheck)
        {
            //Move 
            transform.position = Vector3.Lerp(this.transform.position, tragetMovePos[isCount].position, Time.deltaTime);
            //transform.LookAt(tragetMovePos[isCount]);
        }
        if(isLook)
        {
            transform.LookAt(player);
        }
        
        checkMove();
    }

    IEnumerator waitTime() //wait go time
    {
        yield return new WaitForSeconds(waitTimeValue);

        if(isGuideCheck == true)
        {
            isCheckMove = true;
            
        }            
    }



    void checkMove()
    {
        float checkDistance = (this.transform.position - tragetMovePos[isCount].position).magnitude;//check
        
        if (checkDistance < 0.1f) //check move
        {
            isGuideCheck = false;
            
            if (isCount < listLength - 1)
            {
                isCount += 1;
                
            }

        }

    }



    void movePos(bool getCheckMove) //move check
    {
        //Debug.Log(isCount);
        float checkDistance = (this.transform.position - tragetMovePos[isCount].position).magnitude;//check
        //Debug.Log(checkDistance);

        if (checkDistance < 0.1f) //check move
        {//pickUpType = "sitIdle";
            
            isCheckMove = false;
            if(isCount < listLength-1)
            {
                isCount += 1;
                
                StartCoroutine(waitTime());
            }
        }
        if (isCheckMove==true)
        {
            //Debug.Log(isCheckMove);
            StopCoroutine(waitTime());
            transform.position = Vector3.Lerp(this.transform.position, tragetMovePos[isCount].position, Time.deltaTime);
            transform.LookAt(tragetMovePos[isCount]);
            
        }

    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "GuideCheck") 
        {
            isGuideCheck = true;
        }
    }
    

}
