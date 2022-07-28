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
    public bool isAnim;
    public Animator _Animator;
    private bool isListNull;
    public GameObject DestoryObj;
    private void Start()
    {
        if (isAnim)
        {
            _Animator = _Animator.GetComponent<Animator>();
        }
        if (showObj.Length == 0)
        {
            isListNull = true;
        }
    }

    private void Update()
    {
        if (!isCheckSel) return;

        
        if (menuCheck.targetItemHeld == checkHeld)
        {
            if (isAnim)
            {
                ShowAnim();
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.1f);

            }

            if (!isListNull)
            {

                for (int i = 0; i < showObj.Length; i++)
                {
                    showObj[i].SetActive(true);
                }

                for (int i = 0; i < hideList.Length; i++)
                {
                    hideList[i].SetActive(false);
                }

                //Destroy(gameObject, 3);
            }

        }
    }
    public void DestoryObjFun()
    {
        Destroy(DestoryObj, 2f);
    }
    public void AddItemHeld()
    {
        menuCheck.targetItemHeld = checkHeld;
        isCheckSel = true;
    }
    public void ShowAnim()
    {
          _Animator.SetTrigger("isGet");
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
