using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.XR.CoreUtils;
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
           //baseValue = getCollider.radius;
           //getCollider.radius = 0;
           isTrigger = true;
        }
        

    }
    [Button]
    public void AddItemHeld(){
        menuCheck.targetItemHeld = checkHeld;
        var childRoot = gameObject.GetNamedChild("targetObjLocal");
        var childrenList = childRoot.GetComponentsInChildren<Transform>(false).ToList();
        childrenList.RemoveAt(0);
        var child = childrenList.First();
        var childrenName = child.name;
        menuCheck.targetItemName = childrenName;
    }
    public void ShowAnim()
    {
          _Animator.SetBool("isGet",true);
          _Animator.SetTrigger("isGet");
    }
    private void OnTriggerEnter(Collider other)
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
            if (isHandTouch)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.02f);
                getCollider.radius += countTimer;
            }
            if (rayAnimator != null)
            {
                rayAnimator.SetBool("isGet",true);
            }
            if (countTimer > coldTime)
            {   
                AddItemHeld();
                ShowAnim();
                StartCoroutine(WaitTimeScaleCollider());                
            }
        }
    }
    IEnumerator WaitTimeScaleCollider()
    {
        yield return new WaitForSeconds(2);
        LoadTimeLine();
    }
    public void LoadTimeLine()
    {
        showObj[0].SetActive(true);
        hideList[0].SetActive(false);
        gameObject.transform.parent.gameObject.SetActive(false);
        isTrigger = false;
        countTimer = 0;
    }
}
