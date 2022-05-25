using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PropRealLibrary
{
    [CreateAssetMenu(fileName = "NewPropRealLibray", menuName = "HelloPico/New PropRealLibray")]
    public class PropReal_Inventory_SO : ScriptableObject
    {
        public GameObject[] ProprealList;
    }
}

