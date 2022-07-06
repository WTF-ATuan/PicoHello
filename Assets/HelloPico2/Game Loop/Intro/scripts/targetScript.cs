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
    public int coldTime;
    float countTimer;
    
    

    private void Update()
    {
        if (!isCheckSel) return;

        
        if (menuCheck.targetItemHeld == checkHeld)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.1f);


            for (int i=0; i < showObj.Length; i++)
            {
                showObj[i].SetActive(true);
            }

            for (int i = 0; i < hideList.Length; i++)
            {
                hideList[i].SetActive(false);
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
        if (isTrigger && other.CompareTag("Player")&&coldTime==0)
        {   
            menuCheck.targetItemHeld = checkHeld;
            isCheckSel = true;
            countTimer = 0;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (isTrigger && other.CompareTag("Player"))
        {
            countTimer += Time.deltaTime;
            
            if (countTimer > coldTime)
            {
                menuCheck.targetItemHeld = checkHeld;
                isCheckSel = true;
                countTimer = 0;
            }
        }
    }
}
