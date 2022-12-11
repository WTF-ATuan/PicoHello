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
    public float coldTime;
    float countTimer;
    public bool isAnim;
    public Animator _Animator;
    public Animator rayAnimator;
    private bool isListNull;
    SphereCollider getCollider;
    public bool isHandTouch;
    float baseValue;
    public bool isAudio;
    public float isAudioDelay;
    public GameObject aduioObj;
    public bool isEffect;
    public GameObject EffectObj;
    public bool isStaff;
    public bool touchEffect;
    public GameObject touchEffectObj;
    public bool isGetItem;
    public bool isMenuAnim;
    public Animator menuAnimator;
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
        if (menuAnimator) menuAnimator = menuAnimator.GetComponent<Animator>();

    }

    [Button]
    private void Test(){
        AddItemHeld();
        ShowAnim();
        LoadTimeLine();
    }

    public void AddItemHeld(){
        menuCheck.targetItemHeld = checkHeld;
        var childRoot = gameObject.GetNamedChild("targetObjLocal");
        var childrenList = childRoot.GetComponentsInChildren<Transform>(false).ToList();
        childrenList.RemoveAt(0);
        var child = childrenList.First();
        var childrenName = child.name;
        if(menuCheck.ContainTarget(childrenName)){
            menuCheck.targetItemName = childrenName;
        }
    }
    public void ShowAnim()
    {
        
        _Animator.SetBool("isGet", true);
        
        _Animator.SetTrigger("isGet");
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isHandTouch && isTrigger && other.CompareTag("Player"))
        {

            AddItemHeld();
            ShowAnim();
            LoadTimeLine();
            if (isAudio && isAudioDelay==0)
            {
                aduioObj.SetActive(true);
            }
        }
        if (touchEffect)
        {
            touchEffectObj.SetActive(true);
        }
        if (isMenuAnim)
        {
            menuAnimator.SetTrigger("isGet");
            menuAnimator.SetBool("notHold",false);
        }
    }
    private void OnTriggerStay(Collider other)
    {

        countTimer += Time.deltaTime;

        
        if (isAudio && isGetItem &&  isAudioDelay ==0)
        {
            aduioObj.SetActive(true);
        }
        if (isHandTouch && isTrigger && other.CompareTag("Player"))
        {

            if (isHandTouch)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.02f);
                getCollider.radius += countTimer;
            }

            if (countTimer > coldTime)
            {   
                AddItemHeld();
                ShowAnim();

                if (isEffect)
                {
                    EffectObj.SetActive(true);
                }
                
                StartCoroutine(WaitTimeScaleCollider());                
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (isMenuAnim)
        {
            menuAnimator.SetBool("notHold", true);
            if (touchEffect)
            {
                touchEffectObj.SetActive(false);
            }
        }
    }
    IEnumerator WaitTimeScaleCollider()
    {

        if (isAudio)
        {
            aduioObj.SetActive(true);            
        }

        yield return new WaitForSeconds(isAudioDelay);

        LoadTimeLine();
    }
    

    public void LoadTimeLine()
    {
        if (isAudio)
        {
            aduioObj.SetActive(true);
        }
        if (isEffect)
        {
            EffectObj.SetActive(false);
        }
        if (!isStaff)
        {
            gameObject.transform.parent.gameObject.SetActive(false);
        }
        isTrigger = false;

        Invoke("ShowElement", isAudioDelay);

        countTimer = 0;
    }
    void ShowElement()
    {
        showObj[0].SetActive(true);
        hideList[0].SetActive(false);
    }
}
