using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class targetItemAdd_Script : MonoBehaviour
{
    public TargetItem_SO menuCheck;
    public int checkHeld;
    public GameObject[] showObj;
    public GameObject[] hideList;
    public void AddItemHeld()
    {
        menuCheck.targetItemHeld += 1;
        Debug.Log(menuCheck.targetItemHeld);
    }
    private void OnTriggerEnter(Collider other)
    {
         AddItemHeld();
    }
    public void CheckHeldPoint()
    {
        //�˴��O�_�F��ާ@ ToNext 
        if (menuCheck.targetItemHeld >= checkHeld)
        {
            showObj[0].SetActive(true);
            hideList[0].SetActive(false);
            menuCheck.targetItemHeld = checkHeld;

        }
        //showObj[1]�`���о�
        else
        {
            showObj[1].SetActive(true);
            hideList[0].SetActive(false);
        }
    }

}
