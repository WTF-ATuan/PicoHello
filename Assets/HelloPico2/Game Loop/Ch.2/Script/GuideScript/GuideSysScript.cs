using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum guidType {LOOP,GUIDE,ATTACK,ASSIST, CHEER }
public class GuideSysScript : MonoBehaviour
{
    public GuideSys_SO _guideSys;
    public guidType _guidType;
    public GameObject[] guideList;
    int getRandom;
    public Animator _guideAnimator;
    public Animator _energyAnimator;
    public GameObject energryPrefab;
    public GameObject createEnergyPos;
    public float coldTime;
    float timer;
    public bool isTimer;
    
    GameObject ballPool;
    // Start is called before the first frame update
    void Start()
    {
        ballPool = GameObject.Find("ballPool");
        timer = coldTime;
        //Random Guide
        getRandom = Random.Range(0, guideList.Length);
        guideList[getRandom].SetActive(true);

        _guideAnimator = _energyAnimator.GetComponent<Animator>();
        _energyAnimator = _energyAnimator.GetComponent<Animator>();
    }
    void guideSwitchStates()
    {
        switch (_guidType)
        {
            case guidType.LOOP:
                _energyAnimator.SetBool("isAttack", false);
                //_guideAnimator.SetBool()
                break;
            case guidType.GUIDE:
                break;
            case guidType.ATTACK:
                _energyAnimator.SetBool("isAttack", true);
                isTimer = true;
                
                break;
            case guidType.ASSIST:
                Instantiate(energryPrefab, transform.position, Quaternion.identity,ballPool.transform);
                _guideSys.guidesType = 0;
                _guidType = guidType.LOOP;
                break;
            //case guidType
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isTimer == true)
        {
            timer += Time.deltaTime;
            if (timer > coldTime)
            {
                _guideSys.guidesType = 0;
                _guidType = guidType.LOOP;
                timer = 0;
                isTimer = false;
            }
        }
        checkType();
        guideSwitchStates();
    }

    void checkType()
    {
        if (_guideSys.guidesType == 3)
        {
            _guidType = guidType.ASSIST;
        }
        else if (_guideSys.guidesType == 2)
        {
            _guidType = guidType.ATTACK;
        }
    }
 }

