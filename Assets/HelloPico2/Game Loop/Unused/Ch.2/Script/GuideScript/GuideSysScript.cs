using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum guidType {LOOP,GUIDE,ATTACK,ASSIST, CHEER ,DEAD,SHOCK }
public class GuideSysScript : MonoBehaviour
{
    public GuideSys_SO _guideSys;
    public guidType _guidType;
    public GameObject[] guideList;
    int getRandom;
    public AudioClip[] _AudioClip;
    AudioSource _AudioSource;
    public Animator _guideAnimator;
    public Animator _energyAnimator;
    public GameObject energryPrefab;
    public GameObject createEnergyPos;
    public float coldTime;
    public GameObject[]  effectList;
    float timer=0;
    public bool isTimer;
    bool isCreate;
    
    
    GameObject ballPool;
    // Start is called before the first frame update
    void Start()
    {
        ballPool = GameObject.Find("ballPool");
        
        //Random Guide
        getRandom = Random.Range(0, guideList.Length);
        guideList[getRandom].SetActive(true);

        _guideAnimator = _guideAnimator.GetComponent<Animator>();
        _energyAnimator = _energyAnimator.GetComponent<Animator>();
        
        _AudioSource = this.GetComponent<AudioSource>();
    }
    void guideSwitchStates()
    {
        switch (_guidType)
        {
            case guidType.LOOP:
                _energyAnimator.SetBool("isAttack", false);

                _guideAnimator.SetBool("isLook", true);
                _guideAnimator.SetBool("isAttack", false);
                _guideAnimator.SetBool("isAssist", false);
                _guideAnimator.SetBool("isCheer", false);
                _guideAnimator.SetBool("isShock", false);
                _guideAnimator.SetBool("isDead", false);
                effectList[0].SetActive(false);
                effectList[1].SetActive(false);
                effectList[2].SetActive(false);
                //_guideAnimator.SetBool()
                break;
            case guidType.GUIDE:
                break;

            case guidType.ATTACK:
                _guideAnimator.SetBool("isLook", false);
                _guideAnimator.SetBool("isAttack", true);
                _energyAnimator.SetBool("isAttack", true);
                coldTime = 1;
                isTimer = true;
                AudioPlayer(0);
                break;

            case guidType.ASSIST:
                _guideAnimator.SetBool("isLook", false);
                _guideAnimator.SetBool("isAssist", true);
                isCreate = true;
                isTimer = true;
                effectList[1].SetActive(true);
                //_guidType = guidType.LOOP;
                //_guideSys.guidesType = 0;
                break;

            case guidType.CHEER:
                //_guideAnimator.SetBool("isLook", true);
                effectList[0].SetActive(true);
                _guideAnimator.SetBool("isCheer", true);
                coldTime = 1;
                isTimer = true;
                AudioPlayer(2);
                break;


            case guidType.DEAD:
                effectList[2].SetActive(true);
                _guideAnimator.SetBool("isDead", true);
                isTimer = true;

                break;

            case guidType.SHOCK:
                //_guideAnimator.SetBool("isLook", false);
                _guideAnimator.SetBool("isShock", true);
                isTimer = true;
                break;
                //case guidType
        }
    }
    void AudioPlayer(int num)
    {
        _AudioSource.clip = _AudioClip[num];
        _AudioSource.Play();
    }
    // Update is called once per frame
    void Update()
    {
        checkType();
        guideSwitchStates();

        if (isTimer == true)
        {
            timer += Time.deltaTime;
            if (timer > coldTime && isCreate)
            {

                Instantiate(energryPrefab, createEnergyPos.transform.position, Quaternion.identity, ballPool.transform);
                AudioPlayer(1);
                isCreate = false;
                isTimer = false;
                timer = 0;
                _guideSys.guidesType = 0;
                _guidType = guidType.LOOP;
            }
            if(timer > coldTime)
            {
                
                isTimer = false;
                timer = 0;
                _guideSys.guidesType = 0;
                _guidType = guidType.LOOP;
                
            }

        }

    }


    void checkType()
    {
        if (_guideSys.guidesType == 2)
        {
            _guidType = guidType.ATTACK;
        }
        else if (_guideSys.guidesType == 3)
        {
            _guidType = guidType.ASSIST;
        }
        else if (_guideSys.guidesType == 4)
        {
            _guidType = guidType.CHEER;
        }
        else if (_guideSys.guidesType == 5)
        {
            _guidType = guidType.DEAD;
        }
        else if (_guideSys.guidesType == 6)
        {
            _guidType = guidType.SHOCK;
        }
    }
 }

