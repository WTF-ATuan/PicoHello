using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandToGuide { ATTACK, ASSIST, CHECK, CHEER, GET }
public class TargetItemCheckScript : MonoBehaviour
{
    public int itemHeldCheck;
    public CommandToGuide _commandToGuide;
    public TargetItem_SO _targetItem;
    public GameObject showTarget;
    public Animator _animator;
    public float coldTime;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        _animator = _animator.GetComponent<Animator>();
        timer = coldTime;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(_targetItem.targetItemHeld== itemHeldCheck)
        {
            timer -= Time.deltaTime;
            
            if (timer < 0)
            {
                _commandToGuide = CommandToGuide.GET;
            }
        }
        switchContronl();
    }
    void switchContronl()
    {
        switch (_commandToGuide)
        {
            case CommandToGuide.CHECK:
                break;

            case CommandToGuide.GET:
                _animator.SetBool("isCheer", false);
                _animator.SetBool("isAssist", false);
                showTarget.SetActive(true);
                _targetItem.targetItemHeld = 0;
                this.gameObject.SetActive(false);

                break;
            case CommandToGuide.ASSIST:
                _animator.SetBool("isAssist",true);
                break;

            case CommandToGuide.CHEER:
                _animator.speed = 2.0f;
                _animator.SetBool("isCheer", true);
                break;
        }
            

    }
}
