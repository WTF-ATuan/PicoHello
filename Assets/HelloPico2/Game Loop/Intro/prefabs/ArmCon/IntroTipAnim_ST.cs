using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroTipAnim_ST : MonoBehaviour
{
    
    public TargetItem_SO _introItem;
    public Animator _PosAnimator;
    public Animator _ConAnimator;
    public GameObject getTipGrp;
    public GameObject[] btnMeshList;
    public GameObject AudioOpen;
    float timer;
    public float ColdTime=3;
    int isNotGetCounter=0;
    int isTipTyp;
    // Start is called before the first frame update
    void Start()
    {
        _PosAnimator = _PosAnimator.GetComponent<Animator>();
        _ConAnimator = _ConAnimator.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_introItem.isTipArm)
        {
            _PosAnimator.SetBool("isClose", false);
            AudioOpen.SetActive(true);
            SwitchAnim();
        }
        else
        {
            isNotGetCounter += 1;

            getTipGrp.SetActive(false);
            AudioOpen.SetActive(false);
            _PosAnimator.gameObject.SetActive(false);
            _PosAnimator.SetBool("isClose",true);
        }

    }

    void SwitchAnim()
    {
        switch (isTipTyp)
        {
            case 0:
                _PosAnimator.gameObject.SetActive(true);
                _PosAnimator.SetBool("isGrip", true);
                _PosAnimator.SetBool("isTrigget", false);
                getTipGrp.SetActive(true);
                getTipGrp.transform.GetChild(0).gameObject.SetActive(true);
                getTipGrp.transform.GetChild(1).gameObject.SetActive(false);
                btnMeshList[0].SetActive(true);
                btnMeshList[1].SetActive(false);

                timer += Time.deltaTime;
                if (isNotGetCounter < 2)
                {
                    _ConAnimator.SetFloat("Grip", timer);
                }
                else
                {
                    _ConAnimator.SetFloat("Grip", timer*0.2f);
                }

                if (timer > 2)
                {
                    
                    if (isNotGetCounter > 4)
                    {
                        if (timer > 5)
                        {
                            isTipTyp = 1;
                            timer = 0;
                            
                        }
                    }
                    if (isNotGetCounter < 2)
                    {
                        timer = 0;
                    }
                }
                
                //_ConAnimator.SetBool("isGrip",true);
                break;
            case 1:
                _PosAnimator.gameObject.SetActive(true);
                getTipGrp.SetActive(true);
                _PosAnimator.SetBool("isGrip", false);
                _PosAnimator.SetBool("isTrigget", true);

                getTipGrp.transform.GetChild(0).gameObject.SetActive(false);
                getTipGrp.transform.GetChild(1).gameObject.SetActive(true);
                btnMeshList[1].SetActive(true);
                btnMeshList[0].SetActive(false);
                _ConAnimator.SetFloat("Grip", 0);

                timer += Time.deltaTime;
                if (isNotGetCounter < 2)
                {
                    _ConAnimator.SetFloat("Trigget", timer*2f);
                }
                else
                {
                    _ConAnimator.SetFloat("Trigget", timer * 0.4f);
                }

                if (timer > 1)
                {
                    if (isNotGetCounter > 4)
                    {
                        if(timer > 5)
                        {
                            isTipTyp = 0;
                            timer = 0;
                        }
                    }
                    if(isNotGetCounter < 4)
                    {
                        timer = 0;
                    }
                }
                break;

        }
        
        
    }
}
