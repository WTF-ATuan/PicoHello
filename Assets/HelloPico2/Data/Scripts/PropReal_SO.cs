using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPropRealItem", menuName = "HelloPico/New PropRealItem")]
public class PropReal_SO : ScriptableObject
{
    public enum realType { Dilihence, Emotion, Integrity, Knowledge, Mercy }
    public realType _realType;
    public string realName;
    [Range (1,5)]
    public int degree;
}
