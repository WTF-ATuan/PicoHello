using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class targetScript : MonoBehaviour
{
    public TargetItem_SO menuCheck;
    public GameObject[] showObj;
    public GameObject[] hideList;
    public int checkHeld;
    public bool isTrigger;
    public int coldTime;
    float countTimer;
    public bool isAnim;
    public Animator _Animator;
    private bool isListNull;
    
    
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

                Destroy(gameObject, 3);
            }
        }


    }

    public void AddItemHeld()
    {
        menuCheck.targetItemHeld = checkHeld;
        
    }
    public void ShowAnim()
    {
          _Animator.SetBool("isGet",true);
    }
   
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (isTrigger && other.CompareTag("Player")&&coldTime==0)
        {
            AddItemHeld();
        }
    }*/
    private void OnTriggerStay(Collider other)
    {
        if (isTrigger && other.CompareTag("Player"))
        {
            countTimer += Time.deltaTime;
            
            if (countTimer > coldTime)
            {
                //menuCheck.targetItemHeld = checkHeld;
                ShowAnim();
                LoadTimeLine();
            }
        }
    }
    private void LoadTimeLine()
    {
            showObj[0].SetActive(true);
            hideList[0].SetActive(false);
            isTrigger = false;
            countTimer = 0;
    }
}
