using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class targetScript : MonoBehaviour
{
    public TargetItem_SO menuCheck;
    public GameObject[] showObj;
    public GameObject[] hideList;

    public int checkHeld;
    public bool isCheckSel;
    public bool isTrigger;
    bool isShowList;
    bool isHideList;
    
    private void Start()
    {
        if (showObj.Length != 0)
        {
            isShowList = true;
        }
        if (hideList.Length != 0)
        {
            isHideList = true;
        }
    }
    private void Update()
    {
        if (!isCheckSel) return;

        
        if (menuCheck.targetItemHeld == checkHeld)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.1f);

            if (isShowList)
            {
                foreach (GameObject showObjElement in showObj)
                {
                    showObjElement.SetActive(true);
                }
            }
            if(isHideList)
            {
                foreach (GameObject hideObjElement in hideList)
                {
                    hideObjElement.SetActive(false);
                }
            }
            Destroy(gameObject, 3); 
        }
    }
    
    public void AddItemHeld()
    {
        menuCheck.targetItemHeld = checkHeld;
        isCheckSel = true;
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (isTrigger)
        {
            menuCheck.targetItemHeld = checkHeld;
            isCheckSel = true;
        }
    }
}
