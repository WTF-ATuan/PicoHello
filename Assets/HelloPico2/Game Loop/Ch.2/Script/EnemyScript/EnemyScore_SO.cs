using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScoreSO", menuName = "HelloPico/ NewScoreSO")]
public class EnemyScore_SO : ScriptableObject
{
    public string ScoreName;
    public int getStepHit;
    public int getHit;
    public int stepCounter;
}
