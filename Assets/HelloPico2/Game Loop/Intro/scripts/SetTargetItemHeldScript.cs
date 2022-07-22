using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTargetItemHeldScript : MonoBehaviour
{
    public int SetHeldDefault;
    public TargetItem_SO ItemHeld;
    // Start is called before the first frame update
    void Start()
    {
        ItemHeld.targetItemHeld = SetHeldDefault;
    }

}
