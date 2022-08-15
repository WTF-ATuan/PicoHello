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
    public Animator rayAnimator;
    private bool isListNull;
    SphereCollider getCollider;
    public bool isHandTouch;
    float baseValue;
    private void Start()
    {
        countTimer = 0;
        if (rayAnimator != null)
        {
            rayAnimator = rayAnimator.GetComponent<Animator>();
        }
        if (isAnim)
        {
            _Animator = _Animator.GetComponent<Animator>();
        }
        if (showObj.Length == 0)
        {
            isListNull = true;
        }
        if (isHandTouch)
        {
            getCollider = gameObject.GetComponent<SphereCollider>();
            baseValue = getCollider.radius;
            getCollider.radius = 0;
            isTrigger = true;
        }
        

    }
    /*
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


    }*/

    public void AddItemHeld()
    {
        menuCheck.targetItemHeld = checkHeld;
        
    }
    public void ShowAnim()
    {
          _Animator.SetBool("isGet",true);
    }
    private void nTriggerEnter(Collider other)
    {
        if (!isHandTouch && isTrigger && other.CompareTag("Player"))
        {
                AddItemHeld();
                ShowAnim();
                LoadTimeLine();

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (isHandTouch && isTrigger && other.CompareTag("Player"))
        {
            countTimer += Time.deltaTime;
            Debug.Log(countTimer);
            if (rayAnimator != null)
            {
                rayAnimator.SetBool("isGet",true);
            }
            if (countTimer > coldTime)
            {
                if (isHandTouch)
                {
                    getCollider.radius = baseValue;
                }
                AddItemHeld();
                ShowAnim();
                LoadTimeLine();
            }
        }
    }
    public void LoadTimeLine()
    {
            showObj[0].SetActive(true);
            hideList[0].SetActive(false);
            isTrigger = false;
            countTimer = 0;
    }
}
