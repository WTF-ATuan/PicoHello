using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewTargetItem", menuName = "HelloPico/New TargetItem")]
public class TargetItem_SO : ScriptableObject
{
    public string targetItemName;
    public int targetItemHeld;
    
    public bool isTipArm;
    public bool isTipOpen;
    
    public bool isTipR;
    public bool isTipL;

    [TextArea]
    public string itemInfo;

    public void SetTarget(int itemHeld){
        targetItemHeld = itemHeld;
    }

}
