using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetItemCheckScript : MonoBehaviour
{
    public bool isAssist;
    public bool isCheer;
    public bool isAttack;
    public bool isDead;
    public int itemHeldCheck;
    
    public TargetItem_SO _targetItem;
    public GuideSys_SO _guideSys;

    public bool isNext;
    public GameObject ShowNext;
    public float coldTime;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
        timer = coldTime;
        _targetItem.targetItemHeld = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            timer -= Time.deltaTime;
            _guideSys.guidesType = 5;
            if (timer < 0)
            {
                _guideSys.guidesType = 0;
            }
        }
        if (isAttack)
        {
            timer -= Time.deltaTime;
            _guideSys.guidesType = 2;
            if(timer < 0)
            {
                _guideSys.guidesType = 0;
            }
        }
        if(isAssist)
        {
            //timer -= Time.deltaTime;
            _guideSys.guidesType = 3;
            /*
            if (timer < 0)
            {
                _guideSys.guidesType = 0;
                //this.gameObject.SetActive(false);
            }*/
        }
        if(isCheer && _targetItem.targetItemHeld == itemHeldCheck)
        {
            timer -= Time.deltaTime;
            _guideSys.guidesType = 4;

            if (timer < 0)
            {
                _targetItem.targetItemHeld = 2;
                _guideSys.guidesType = 0;
                ShowNext.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }

    }
}
