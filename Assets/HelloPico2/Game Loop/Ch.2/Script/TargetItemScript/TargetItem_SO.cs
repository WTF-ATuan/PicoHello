using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewTargetItem", menuName = "HelloPico/New TargetItem")]
public class TargetItem_SO : ScriptableObject
{
    public string targetIteamName;
    public int targetItemHeld;
    [TextArea]
    public string itemInfo;
}
